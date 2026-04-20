# ----------------------------------------------------------
# Sync-FromProduction.ps1
# Povlači novosti iz produkcijskog Vektor repo-a u VektorTest
# ----------------------------------------------------------

Set-Location "D:\QCS_2022\VektorTest"

# 1. Dohvati sve novosti s produkcijskog remote-a
git fetch production

# 2. Provjeri ima li uncommitted promjena — ako da, stash-aj ih
$dirty = git status --porcelain
if ($dirty) {
    Write-Host "Stash-am lokalne promjene..." -ForegroundColor Yellow
    git stash push -m "auto-stash prije sync s produkcijom $(Get-Date -Format 'yyyy-MM-dd_HH-mm')"
    $wasStashed = $true
}

# 3. Merge produkcijski master u trenutni branch
Write-Host "Merge-am production/master..." -ForegroundColor Cyan
git merge production/master --no-edit

# 4. Ako je bilo konflikata, zaustavi se
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "*** KONFLIKT! Riješi konflikte ručno, zatim 'git merge --continue'." -ForegroundColor Red
    Write-Host "*** Stash-ane promjene čekaju: 'git stash pop' nakon što završiš merge." -ForegroundColor Red
    exit 1
}

# 5. Vrati stash-ane promjene ako ih je bilo
if ($wasStashed) {
    Write-Host "Vraćam stash-ane promjene..." -ForegroundColor Yellow
    git stash pop
}

# 6. Push na origin (VektorTest remote)
Write-Host "Push-am na origin (VektorTest)..." -ForegroundColor Cyan
git push origin

Write-Host "Sync završen." -ForegroundColor Green