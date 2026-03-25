# PowerShell script for backup, compression, encryption, and Dropbox upload
# usage:
# powershell -ExecutionPolicy Bypass -File vvBkpScript_ChatGPT.ps1 ROZEL NULL D:\VIPER\MyVvData D:\VIPER\MyVvBackup D:\VIPER\Dropbox 2026 trtmrkzvrK^01

param(
    [string]$vvBkpID,
    [string]$vvDomena,
    [string]$vvDataDir,
    [string]$vvBkpDir,
    [string]$vvDropBox,
    [string]$vvYear,
    [string]$vvPassword
)

# ================= CONFIG =================
$outgoingMailServer = "mail.htnet.hr"
$fromEmail = "$vvBkpID@zg.htnet.hr"
$toEmail = "viper@zg.htnet.hr"

$logFile = Join-Path $vvBkpDir "backup.log"

# ================= FUNCTIONS =================
function Send-ErrorMail($subject) {
    try {
        Send-MailMessage `
            -SmtpServer $outgoingMailServer `
            -From $fromEmail `
            -To $toEmail `
            -Subject $subject `
            -Body $subject
    } catch {
        Write-Host "Mail send failed: $_"
    }
}

function Exit-WithError($msg) {
    Write-Host "ERROR: $msg"
    Send-ErrorMail $msg
    exit 1
}

# ================= TIME =================
$startTime = Get-Date
$today = Get-Date -Format "yyyyMMdd"

# ================= FILE NAME =================
if ($vvDomena -and $vvDomena -ne "NULL") {
    $filePrefix = "vvBkp_${vvDomena}_${vvBkpID}_${vvYear}_$today"
    $vvektorName = "${vvDomena}_vvektor"
} else {
    $filePrefix = "vvBkp_${vvBkpID}_${vvYear}_$today"
    $vvektorName = "vvektor"
}

$tempDir = Join-Path $vvBkpDir "temp_$today"
New-Item -ItemType Directory -Path $tempDir -Force | Out-Null

# ================= SELECT DATABASE DIRS =================
try {
    $dirs = Get-ChildItem $vvDataDir -Directory | Where-Object {
        $_.Name -like "*$vvYear*" -or
        $_.Name -eq "mysql" -or
        $_.Name -eq $vvektorName
    }

    if (-not $dirs) {
        Exit-WithError "No database directories found"
    }

    foreach ($dir in $dirs) {
        Copy-Item $dir.FullName -Destination $tempDir -Recurse -Force
    }
} catch {
    Exit-WithError "Copy failed: $_"
}

# ================= COMPRESS =================
$archiveFile = Join-Path $vvBkpDir "$filePrefix.7z"

try {
    & 7z a $archiveFile "$tempDir\*" | Out-Null
    if (!(Test-Path $archiveFile)) {
        Exit-WithError "Compression failed"
    }
} catch {
    Exit-WithError "7zip error: $_"
}

# ================= ENCRYPT =================
$encryptedFile = "$archiveFile.enc"

try {
    & dlock2 /E /S "$archiveFile" "$encryptedFile" /P"$vvPassword"
    if (!(Test-Path $encryptedFile)) {
        Exit-WithError "Encryption failed"
    }
    Remove-Item $archiveFile -Force
} catch {
    Exit-WithError "Encryption error: $_"
}

# ================= CLEAN TEMP =================
Remove-Item $tempDir -Recurse -Force

# ================= CLEAN OLD BACKUPS (LOCAL) =================
try {
    $now = Get-Date

    Get-ChildItem $vvBkpDir -Filter "*.enc" | Where-Object {
        $_.LastWriteTime -lt $now.AddDays(-7) -and
        $_.LastWriteTime.DayOfWeek -ne "Friday"
    } | Remove-Item -Force
} catch {
    Send-ErrorMail "Cleanup local failed: $_"
}

# ================= COPY TO DROPBOX =================
$dropFile = Join-Path $vvDropBox ([System.IO.Path]::GetFileName($encryptedFile))

try {
    Copy-Item $encryptedFile $dropFile -Force

    if (!(Test-Path $dropFile)) {
        Exit-WithError "Dropbox copy failed"
    }
} catch {
    Exit-WithError "Dropbox error: $_"
}

# ================= CLEAN DROPBOX =================
try {
    $files = Get-ChildItem $vvDropBox -Filter "*.enc" | Sort-Object LastWriteTime -Descending
    $files | Select-Object -Skip 2 | Remove-Item -Force
} catch {
    Send-ErrorMail "Dropbox cleanup failed: $_"
}

# ================= LOG =================
$endTime = Get-Date
$duration = ($endTime - $startTime).ToString()

$logLine = "$filePrefix | $startTime | $endTime | $duration"
Add-Content -Path $logFile -Value $logLine

Write-Host "Backup completed successfully"