# ----------------------------------------------------------
# Sync-FromProduction.ps1
# Povlači novosti iz produkcijskog Vektor repo-a u VektorTest
# ----------------------------------------------------------

Set-Location "D:\QCS_2022\VektorTest"

# 1. Dohvati sve novosti s produkcijskog remote-a
Write-Host "1/6 - Fetch production..." -ForegroundColor Cyan
git fetch production

# 2. Provjeri ima li uncommitted promjena — ako da, stash-aj ih
$dirty = git status --porcelain
$wasStashed = $false
if ($dirty) {
    Write-Host "2/6 - Stash-am lokalne promjene..." -ForegroundColor Yellow
    git stash push -m "auto-stash prije sync s produkcijom $(Get-Date -Format 'yyyy-MM-dd_HH-mm')"
    $wasStashed = $true
} else {
    Write-Host "2/6 - Nema lokalnih promjena za stash." -ForegroundColor Gray
}

# 3. Provjeri ima li uopce nesto novo za merge
$behind = git rev-list --count "HEAD..production/master"
if ($behind -eq "0") {
    Write-Host "Nema novosti u produkciji. Sve je vec azurno!" -ForegroundColor Green
    if ($wasStashed) { git stash pop }
    exit 0
}
Write-Host "3/6 - Merge-am $behind commit(a) iz production/master..." -ForegroundColor Cyan

# 4. Merge produkcijski master u trenutni branch
git merge production/master --no-edit

# 5. Ako je bilo konflikata, zaustavi se s uputama
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Red
    Write-Host "  KONFLIKT! Slijedi ove korake:            " -ForegroundColor Red
    Write-Host "============================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "  1. U Visual Studio otvori: View -> Git Changes" -ForegroundColor Yellow
    Write-Host "  2. Klikni na datoteku s konfliktom (npr. Vektor.csproj)" -ForegroundColor Yellow
    Write-Host "  3. Otvara se Merge Editor - odaberi sto zadrzati" -ForegroundColor Yellow
    Write-Host "  4. Klikni 'Accept Merge', pa 'Commit' u Git Changes" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  Nakon toga pokreni u terminalu:" -ForegroundColor Cyan
    if ($wasStashed) {
        Write-Host "    git stash pop" -ForegroundColor White
    }
    Write-Host "    git push origin" -ForegroundColor White
    Write-Host ""
    exit 1
}

# 6. Vrati stash-ane promjene ako ih je bilo
if ($wasStashed) {
    Write-Host "5/6 - Vracam stash-ane promjene..." -ForegroundColor Yellow
    git stash pop
}

# 7. Push na origin (VektorTest remote)
Write-Host "6/6 - Push-am na origin (VektorTest)..." -ForegroundColor Cyan
git push origin

Write-Host ""
Write-Host "Sync zavrsena! Preuzeto $behind commit(a) iz produkcije." -ForegroundColor Green