@ECHO off
SETLOCAL EnableDelayedExpansion

:: ======================================================================
:: vvBkpScriptAuto.bat - MySQL Database Backup Script (Auto Year)
:: Usage: vvBkpScriptAuto.bat BkpID Domain DataDir BkpDir DropBox Password
:: Requires: 7-Zip installed at "C:\Program Files\7-Zip\7z.exe"
:: 
:: Automatically backs up current year databases.
:: Before May 1st, also backs up previous year databases.
:: ======================================================================

:: CLS

:: ===== PARSE COMMAND LINE ARGUMENTS =====
SET "vvBkpID=%~1"
SET "vvDomena=%~2"
SET "vvDataDir=%~3"
SET "vvBkpDir=%~4"
SET "vvDropBox=%~5"
SET "vvPassword=%~6"

:: ===== MAIL CONFIGURATION =====
SET "outgoingMailServer=mail.htnet.hr"
SET "fromEmail=%vvBkpID%@zg.htnet.hr"
SET "toEmail=viper@zg.htnet.hr"
SET "qcFextension=vvv"

:: ===== VALIDATE PARAMETERS =====
IF "%vvBkpID%"=="" GOTO :USAGE
IF "%vvDataDir%"=="" GOTO :USAGE
IF "%vvBkpDir%"=="" GOTO :USAGE
IF "%vvDropBox%"=="" GOTO :USAGE
IF "%vvPassword%"=="" GOTO :USAGE

:: ===== CHECK FOR 7-ZIP =====
SET "sevenZip="
IF EXIST "C:\Program Files\7-Zip\7z.exe" SET "sevenZip=C:\Program Files\7-Zip\7z.exe"
IF EXIST "C:\Program Files (x86)\7-Zip\7z.exe" SET "sevenZip=C:\Program Files (x86)\7-Zip\7z.exe"

IF NOT DEFINED sevenZip (
    SET "errorMsg=7-Zip not installed. Please install from https://www.7-zip.org/"
    GOTO :SEND_ERROR
)

:: ===== INITIALIZE LOG FILE EARLY =====
SET "logFile=%vvBkpDir%\vvBkpLog.txt"

:: ===== GET CURRENT DATE/TIME =====
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd"') DO SET "theYYYYMMDD=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "startTime=%%i"
SET "startTimestamp=%DATE% %startTime%"

:: ===== CALCULATE YEAR(S) TO BACKUP =====
FOR /f %%i IN ('powershell -NoProfile -Command "(Get-Date).Year"') DO SET "currentYear=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "(Get-Date).AddYears(-1).Year"') DO SET "previousYear=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "if ((Get-Date) -lt (Get-Date -Year (Get-Date).Year -Month 5 -Day 1)) { 'YES' } else { 'NO' }"') DO SET "backupPrevYear=%%i"

ECHO.
ECHO ======================================================================
ECHO   vvBkpScriptAuto - Starting backup
ECHO   Start time: %startTimestamp%
ECHO   Current year: %currentYear%
IF "%backupPrevYear%"=="YES" (
    ECHO   Previous year: %previousYear% (before May 1st - dual backup)
) ELSE (
    ECHO   Previous year: Not needed (after May 1st)
)
ECHO   Compression: 7-Zip maximum
ECHO ======================================================================
ECHO.

:: ===== VALIDATE DATA DIRECTORY EXISTS =====
IF NOT EXIST "%vvDataDir%" (
    SET "errorMsg=Data directory does not exist: %vvDataDir%"
    GOTO :SEND_ERROR
)

:: ===== BACKUP CURRENT YEAR =====
SET "vvYear=%currentYear%"
CALL :DO_BACKUP
IF !ERRORLEVEL! NEQ 0 GOTO :END

:: ===== BACKUP PREVIOUS YEAR (if before May 1st) =====
IF "%backupPrevYear%"=="YES" (
    ECHO.
    ECHO ----------------------------------------------------------------------
    ECHO   Now backing up previous year: %previousYear%
    ECHO ----------------------------------------------------------------------
    ECHO.
    SET "vvYear=%previousYear%"
    CALL :DO_BACKUP
    IF !ERRORLEVEL! NEQ 0 GOTO :END
)

:: ===== STEP 4: DELETE OLD BACKUPS IN BKPDIR =====
ECHO.
ECHO [CLEANUP] Cleaning old backups in %vvBkpDir%...

SET "cleanupScript=%TEMP%\vvBkpCleanup.ps1"

(
    ECHO $bkpDir = '%vvBkpDir%'
    ECHO $monday = ^(Get-Date^).AddDays(-^(^(Get-Date^).DayOfWeek.value__ - 1^)^).Date
    ECHO.
    ECHO Get-ChildItem -Path $bkpDir -Filter "vvBkp_*.7z.vvv" ^| ForEach-Object {
    ECHO     $name = $_.Name -replace '\.7z\.vvv$', ''
    ECHO     $dateStr = $name.Substring^($name.Length - 8^)
    ECHO     try {
    ECHO         $fileDate = [datetime]::ParseExact^($dateStr, 'yyyyMMdd', $null^)
    ECHO         if ^($fileDate -lt $monday^) {
    ECHO             if ^($fileDate.DayOfWeek -ne 'Friday'^) {
    ECHO                 Write-Host "  Deleting: $^($_.Name^)"
    ECHO                 Remove-Item $_.FullName -Force
    ECHO             } else {
    ECHO                 Write-Host "  Keeping ^(Friday^): $^($_.Name^)"
    ECHO             }
    ECHO         } else {
    ECHO             Write-Host "  Keeping ^(current week^): $^($_.Name^)"
    ECHO         }
    ECHO     } catch {
    ECHO         Write-Host "  Skipping ^(invalid date^): $^($_.Name^)"
    ECHO     }
    ECHO }
) > "%cleanupScript%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%cleanupScript%"
DEL "%cleanupScript%" 2>nul

ECHO [CLEANUP] BkpDir complete.
ECHO.

:: ===== STEP 5: DELETE OLD BACKUPS IN DROPBOX (keep last 2 per year) =====
ECHO [CLEANUP] Cleaning old backups in Dropbox (keeping last 2 per year)...

SET "dropboxCleanupScript=%TEMP%\vvBkpDropboxCleanup.ps1"

(
    ECHO $dropboxDir = '%vvDropBox%'
    ECHO $currentYear = %currentYear%
    ECHO $previousYear = %previousYear%
    ECHO $backupPrevYear = '%backupPrevYear%'
    ECHO.
    ECHO # Function to keep only last 2 files for a given year pattern
    ECHO function Cleanup-YearBackups {
    ECHO     param^([string]$yearPattern^)
    ECHO     $files = Get-ChildItem -Path $dropboxDir -Filter $yearPattern ^| Sort-Object Name -Descending
    ECHO     $count = 0
    ECHO     foreach ^($file in $files^) {
    ECHO         $count++
    ECHO         if ^($count -gt 2^) {
    ECHO             Write-Host "  Deleting: $^($file.Name^)"
    ECHO             Remove-Item $file.FullName -Force
    ECHO         } else {
    ECHO             Write-Host "  Keeping: $^($file.Name^)"
    ECHO         }
    ECHO     }
    ECHO }
    ECHO.
    ECHO Write-Host "  Processing current year ^($currentYear^)..."
    ECHO Cleanup-YearBackups "vvBkp_*_$($currentYear)_*.7z.vvv"
    ECHO.
    ECHO if ^($backupPrevYear -eq 'YES'^) {
    ECHO     Write-Host "  Processing previous year ^($previousYear^)..."
    ECHO     Cleanup-YearBackups "vvBkp_*_$($previousYear)_*.7z.vvv"
    ECHO }
) > "%dropboxCleanupScript%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%dropboxCleanupScript%"
DEL "%dropboxCleanupScript%" 2>nul

ECHO [CLEANUP] Dropbox complete.
ECHO.

:: ===== FINAL SUCCESS =====
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "endTime=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "$s=[timespan]::Parse('%startTime%'); $e=[timespan]::Parse('%endTime%'); ($e - $s).ToString('hh\:mm\:ss')"') DO SET "totalDuration=%%i"

ECHO ======================================================================
ECHO   ALL BACKUPS COMPLETED SUCCESSFULLY!
ECHO   Total duration: %totalDuration%
IF "%backupPrevYear%"=="YES" (
    ECHO   Years backed up: %currentYear%, %previousYear%
) ELSE (
    ECHO   Year backed up: %currentYear%
)
ECHO ======================================================================
ECHO.

GOTO :END

:: ========================================================================
:: SUBROUTINE: DO_BACKUP - Performs backup for a single year
:: Uses: vvYear, vvBkpID, vvDomena, vvDataDir, vvBkpDir, vvDropBox, vvPassword
:: ========================================================================
:DO_BACKUP

:: Build backup filename
IF /I "%vvDomena%"=="NULL" (
    SET "bkpFileName=vvBkp_%vvBkpID%_%vvYear%_%theYYYYMMDD%"
    SET "vvektorDB=vvektor"
) ELSE (
    SET "bkpFileName=vvBkp_%vvDomena%_%vvBkpID%_%vvYear%_%theYYYYMMDD%"
    SET "vvektorDB=%vvDomena%_vvektor"
)

SET "tempBkpDir=%vvBkpDir%\temp_%vvYear%_%theYYYYMMDD%"
SET "archiveFile=%vvBkpDir%\%bkpFileName%.7z"
SET "encryptedFile=%vvBkpDir%\%bkpFileName%.7z.%qcFextension%"

:: Track database copy count
SET "dbCopyCount=0"

ECHO [%vvYear%] Starting backup: %bkpFileName%
ECHO.

:: ===== STEP 1: COPY DATABASE DIRECTORIES =====
ECHO [%vvYear% STEP 1] Copying database directories...

IF EXIST "%tempBkpDir%" RD /S /Q "%tempBkpDir%"
MD "%tempBkpDir%"
IF ERRORLEVEL 1 (
    SET "errorMsg=Failed to create temp directory: %tempBkpDir%"
    EXIT /B 1
)

:: Copy mysql database (required)
IF EXIST "%vvDataDir%\mysql" (
    XCOPY "%vvDataDir%\mysql" "%tempBkpDir%\mysql\" /E /I /Q /Y
    IF ERRORLEVEL 1 (
        SET "errorMsg=Error copying mysql database"
        EXIT /B 1
    )
    ECHO   - mysql [OK]
    SET /A "dbCopyCount+=1"
) ELSE (
    SET "errorMsg=Required directory not found: %vvDataDir%\mysql"
    EXIT /B 1
)

:: Copy vvektor database (required)
IF EXIST "%vvDataDir%\%vvektorDB%" (
    XCOPY "%vvDataDir%\%vvektorDB%" "%tempBkpDir%\%vvektorDB%\" /E /I /Q /Y
    IF ERRORLEVEL 1 (
        SET "errorMsg=Error copying %vvektorDB% database"
        EXIT /B 1
    )
    ECHO   - %vvektorDB% [OK]
    SET /A "dbCopyCount+=1"
) ELSE (
    SET "errorMsg=Required directory not found: %vvDataDir%\%vvektorDB%"
    EXIT /B 1
)

:: Copy all databases for the specified year
IF /I "%vvDomena%"=="NULL" (
    SET "yearPattern=vv%vvYear%_*"
) ELSE (
    SET "yearPattern=%vvDomena%_vv%vvYear%_*"
)

SET "yearDbCount=0"
FOR /D %%d IN ("%vvDataDir%\!yearPattern!") DO (
    SET "dbName=%%~nxd"
    XCOPY "%%d" "%tempBkpDir%\!dbName!\" /E /I /Q /Y
    IF ERRORLEVEL 1 (
        SET "errorMsg=Error copying !dbName! database"
        EXIT /B 1
    )
    ECHO   - !dbName! [OK]
    SET /A "dbCopyCount+=1"
    SET /A "yearDbCount+=1"
)

:: Verify at least one year database was found
IF !yearDbCount! EQU 0 (
    SET "errorMsg=No databases found matching pattern: !yearPattern!"
    EXIT /B 1
)

:: Get uncompressed size
FOR /f %%i IN ('powershell -NoProfile -Command "(Get-ChildItem -Path '%tempBkpDir%' -Recurse | Measure-Object -Property Length -Sum).Sum"') DO SET "sizeBeforeBytes=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "[math]::Round((Get-ChildItem -Path '%tempBkpDir%' -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB, 2)"') DO SET "sizeBeforeMB=%%i"

ECHO [%vvYear% STEP 1] Complete. Copied !dbCopyCount! database(s).
ECHO   Uncompressed size: %sizeBeforeMB% MB
ECHO.

:: ===== STEP 2: CREATE COMPRESSED ARCHIVE (7-ZIP) =====
ECHO [%vvYear% STEP 2] Creating compressed archive...

IF EXIST "%archiveFile%" DEL "%archiveFile%"

"%sevenZip%" a -t7z -mx=9 -mfb=64 -md=32m "%archiveFile%" "%tempBkpDir%\*"
SET "compressExitCode=!ERRORLEVEL!"

IF !compressExitCode! NEQ 0 (
    SET "errorMsg=7-Zip compression failed with exit code !compressExitCode!"
    EXIT /B 1
)

IF NOT EXIST "%archiveFile%" (
    SET "errorMsg=Error creating 7z archive: %archiveFile%"
    EXIT /B 1
)

:: Get compressed size and calculate ratio
FOR /f %%i IN ('powershell -NoProfile -Command "(Get-Item '%archiveFile%').Length"') DO SET "sizeAfterBytes=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "[math]::Round((Get-Item '%archiveFile%').Length / 1MB, 2)"') DO SET "sizeAfterMB=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "if (%sizeAfterBytes% -gt 0) { [math]::Round(%sizeBeforeBytes% / %sizeAfterBytes%, 2) } else { 0 }"') DO SET "compressionRatio=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "if (%sizeBeforeBytes% -gt 0) { [math]::Round(%sizeAfterBytes% / %sizeBeforeBytes% * 100, 1) } else { 0 }"') DO SET "percentOfOriginal=%%i"

:: Cleanup temp directory
RD /S /Q "%tempBkpDir%"

ECHO [%vvYear% STEP 2] Complete: %archiveFile%
ECHO   Compressed size: %sizeAfterMB% MB
ECHO   Compression: %compressionRatio%:1 (%percentOfOriginal%%% of original)
ECHO.

:: ===== STEP 3: ENCRYPT WITH DLOCK2 =====
ECHO [%vvYear% STEP 3] Encrypting backup file...

IF EXIST "%encryptedFile%" DEL "%encryptedFile%"

dlock2 /E /S "%archiveFile%" "%encryptedFile%" /P"%vvPassword%"
SET "dlockExitCode=!ERRORLEVEL!"

IF !dlockExitCode! NEQ 0 (
    SET "errorMsg=dlock2 encryption failed with exit code !dlockExitCode!"
    EXIT /B 1
)

IF NOT EXIST "%encryptedFile%" (
    SET "errorMsg=Error encrypting file: %encryptedFile%"
    EXIT /B 1
)

:: Delete unencrypted file
DEL "%archiveFile%"

:: Get final encrypted file size
FOR /f %%i IN ('powershell -NoProfile -Command "[math]::Round((Get-Item '%encryptedFile%').Length / 1MB, 2)"') DO SET "sizeFinalMB=%%i"

ECHO [%vvYear% STEP 3] Complete: %encryptedFile%
ECHO   Final size: %sizeFinalMB% MB
ECHO.

:: ===== STEP 4: COPY TO DROPBOX =====
ECHO [%vvYear% STEP 4] Copying to Dropbox...

IF NOT EXIST "%vvDropBox%" (
    SET "errorMsg=Dropbox directory does not exist: %vvDropBox%"
    EXIT /B 1
)

XCOPY "%encryptedFile%" "%vvDropBox%\" /V /Y /F
IF ERRORLEVEL 1 (
    SET "errorMsg=XCOPY to Dropbox failed"
    EXIT /B 1
)

SET "copiedBkpFile=%vvDropBox%\%bkpFileName%.7z.%qcFextension%"

IF NOT EXIST "%copiedBkpFile%" (
    SET "errorMsg=Error copying to Dropbox: %copiedBkpFile%"
    EXIT /B 1
)

ECHO [%vvYear% STEP 4] Complete.
ECHO.

:: ===== STEP 5: WRITE TO LOG FILE =====
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "endTimeYear=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "$s=[timespan]::Parse('%startTime%'); $e=[timespan]::Parse('%endTimeYear%'); ($e - $s).ToString('hh\:mm\:ss')"') DO SET "durationYear=%%i"

IF "%durationYear%"=="" SET "durationYear=00:00:00"

ECHO %theYYYYMMDD%	%bkpFileName%	Start: %startTime%	End: %endTimeYear%	Duration: %durationYear%	Size: %sizeBeforeMB%MB-^>%sizeAfterMB%MB (%percentOfOriginal%%%)	OK >> "%logFile%"

ECHO [%vvYear%] BACKUP COMPLETE: %bkpFileName%.7z.%qcFextension%
ECHO   Size: %sizeBeforeMB% MB -^> %sizeFinalMB% MB (%percentOfOriginal%%% of original)
ECHO.

EXIT /B 0

:: ===== ERROR HANDLERS =====
:SEND_ERROR
ECHO.
ECHO ======================================================================
ECHO   BACKUP FAILED!
ECHO   ERROR: %errorMsg%
ECHO ======================================================================
ECHO.

:: Cleanup temp directories if they exist
IF EXIST "%vvBkpDir%\temp_*" (
    FOR /D %%d IN ("%vvBkpDir%\temp_*") DO RD /S /Q "%%d"
)

:: Get end time for logging
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "endTime=%%i"

:: Write error to log file
IF "%bkpFileName%"=="" SET "bkpFileName=UNKNOWN"
ECHO %theYYYYMMDD%	%bkpFileName%	Start: %startTime%	End: %endTime%	ERROR: %errorMsg% >> "%logFile%"

:: Send email notification
blat -subject "[vvBkp ERROR] %vvBkpID%: %errorMsg%" -body "Backup: %bkpFileName%\nError: %errorMsg%\nTime: %DATE% %endTime%" -server %outgoingMailServer% -f %fromEmail% -to %toEmail%

EXIT /B 1

:USAGE
ECHO.
ECHO Usage: vvBkpScriptAuto.bat BkpID VvDomena DataDir BkpDir DropBox Password
ECHO.
ECHO Example: vvBkpScriptAuto.bat ROZEL NULL D:\VIPER\MyVvData D:\VIPER\MyVvBackup D:\VIPER\Dropbox myPassword
ECHO.
ECHO Note: Year is automatically determined from current date.
ECHO       Before May 1st, both current and previous year are backed up.
ECHO.
ECHO Requires: 7-Zip installed at "C:\Program Files\7-Zip\7z.exe"
ECHO.
EXIT /B 1

:END
ENDLOCAL
EXIT /B 0