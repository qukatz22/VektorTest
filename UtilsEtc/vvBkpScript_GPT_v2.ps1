# PowerShell script for backup, compression, encryption, and Dropbox upload
# usage:
# powershell -ExecutionPolicy Bypass -File vvBkpScript_ChatGPT_v2.ps1 ROZEL NULL D:\VIPER\MyVvData D:\VIPER\MyVvBackup D:\VIPER\Dropbox 2026 trtmrkzvrK^01

param(
    [string]$vvBkpID,
    [string]$vvDomena,
    [string]$vvDataDir,   # više se NE koristi (ostavljen radi kompatibilnosti)
    [string]$vvBkpDir,
    [string]$vvDropBox,
    [string]$vvYear,
    [string]$vvPassword
)

# ================= CONFIG =================
$mysqlUser = "root"
$mysqlPass = ""
$mysqlHost = "127.0.0.1"
$threads = 8

$outgoingMailServer = "mail.htnet.hr"
$fromEmail = "$vvBkpID@zg.htnet.hr"
$toEmail = "viper@zg.htnet.hr"

$logFile = Join-Path $vvBkpDir "backup.log"

# ================= FUNCTIONS =================
function Send-ErrorMail($msg) {
    try {
        Send-MailMessage -SmtpServer $outgoingMailServer -From $fromEmail -To $toEmail -Subject $msg -Body $msg
    } catch {}
}

function Fail($msg) {
    Write-Host "ERROR: $msg"
    Send-ErrorMail $msg
    exit 1
}

# ================= TIME =================
$startTime = Get-Date
$today = Get-Date -Format "yyyyMMdd"

# ================= NAMING =================
if ($vvDomena -and $vvDomena -ne "NULL") {
    $filePrefix = "vvBkp_${vvDomena}_${vvBkpID}_${vvYear}_$today"
    $vvektor = "${vvDomena}_vvektor"
} else {
    $filePrefix = "vvBkp_${vvBkpID}_${vvYear}_$today"
    $vvektor = "vvektor"
}

$tempDump = Join-Path $env:TEMP "mydump_$today"
Remove-Item $tempDump -Recurse -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $tempDump | Out-Null

# ================= GET DATABASE LIST =================
try {
    $dbs = & mysql -u $mysqlUser "-p$mysqlPass" -h $mysqlHost -e "SHOW DATABASES;" |
        Select-Object -Skip 1

    $filtered = $dbs | Where-Object {
        $_ -eq "mysql" -or
        $_ -eq $vvektor -or
        $_ -like "*$vvYear*"
    }

    if (-not $filtered) {
        Fail "No databases found"
    }
} catch {
    Fail "DB list failed: $_"
}

# ================= MYDUMPER =================
try {
    $dbList = ($filtered -join ",")
    
    & mydumper `
        -u $mysqlUser `
        -p $mysqlPass `
        -h $mysqlHost `
        -B $dbList `
        -o $tempDump `
        -t $threads `
        --compress `
        --trx-consistency-only `
        --less-locking `
        --build-empty-files `
        --routines `
        --events

    if (!(Get-ChildItem $tempDump)) {
        Fail "Mydumper produced no files"
    }
} catch {
    Fail "Mydumper failed: $_"
}

# ================= COMPRESS + ENCRYPT =================
$archive = Join-Path $vvBkpDir "$filePrefix.7z"

try {
    & 7z a $archive "$tempDump\*" `
        -mx=9 `
        -mhe=on `
        -p"$vvPassword" | Out-Null

    if (!(Test-Path $archive)) {
        Fail "7zip failed"
    }
} catch {
    Fail "Compression failed: $_"
}

# ================= CLEAN TEMP =================
Remove-Item $tempDump -Recurse -Force

# ================= CLEAN LOCAL =================
try {
    $now = Get-Date

    Get-ChildItem $vvBkpDir -Filter "*.7z" | Where-Object {
        $_.LastWriteTime -lt $now.AddDays(-7) -and
        $_.LastWriteTime.DayOfWeek -ne "Friday"
    } | Remove-Item -Force
} catch {
    Send-ErrorMail "Local cleanup failed"
}

# ================= COPY TO DROPBOX =================
$dropFile = Join-Path $vvDropBox ([IO.Path]::GetFileName($archive))

try {
    Copy-Item $archive $dropFile -Force

    if (!(Test-Path $dropFile)) {
        Fail "Dropbox copy failed"
    }
} catch {
    Fail "Dropbox error: $_"
}

# ================= CLEAN DROPBOX =================
try {
    $files = Get-ChildItem $vvDropBox -Filter "*.7z" | Sort-Object LastWriteTime -Descending
    $files | Select-Object -Skip 2 | Remove-Item -Force
} catch {
    Send-ErrorMail "Dropbox cleanup failed"
}

# ================= LOG =================
$endTime = Get-Date
$duration = ($endTime - $startTime)

$logLine = "$filePrefix | $startTime | $endTime | $duration"
Add-Content $logFile $logLine

Write-Host "✅ BACKUP OK: $filePrefix"