@ECHO off
SETLOCAL EnableDelayedExpansion

:: ======================================================================
:: vvBkpScript.bat - MySQL Database Backup Script
:: Usage: vvBkpScript.bat BkpID Domain DataDir BkpDir DropBox Year Password
:: Requires: 7-Zip installed at "C:\Program Files\7-Zip\7z.exe"
:: ======================================================================

:: CLS

:: ===== PARSE COMMAND LINE ARGUMENTS =====
SET "vvBkpID=%~1"
SET "vvDomena=%~2"
SET "vvDataDir=%~3"
SET "vvBkpDir=%~4"
SET "vvDropBox=%~5"
SET "vvYear=%~6"
SET "vvPassword=%~7"

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
IF "%vvYear%"=="" GOTO :USAGE
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

:: ===== BUILD BACKUP FILENAME =====
IF /I "%vvDomena%"=="NULL" (
    SET "bkpFileName=vvBkp_%vvBkpID%_%vvYear%_%theYYYYMMDD%"
    SET "vvektorDB=vvektor"
) ELSE (
    SET "bkpFileName=vvBkp_%vvDomena%_%vvBkpID%_%vvYear%_%theYYYYMMDD%"
    SET "vvektorDB=%vvDomena%_vvektor"
)

SET "tempBkpDir=%vvBkpDir%\temp_%theYYYYMMDD%"
SET "archiveFile=%vvBkpDir%\%bkpFileName%.7z"
SET "encryptedFile=%vvBkpDir%\%bkpFileName%.7z.%qcFextension%"

:: ===== VALIDATE DATA DIRECTORY EXISTS =====
IF NOT EXIST "%vvDataDir%" (
    SET "errorMsg=Data directory does not exist: %vvDataDir%"
    GOTO :SEND_ERROR
)

:: ===== TRACK DATABASE COPY COUNT =====
SET "dbCopyCount=0"

ECHO.
ECHO ======================================================================
ECHO   vvBkpScript - Starting backup: %bkpFileName%
ECHO   Start time: %startTimestamp%
ECHO   Compression: 7-Zip maximum
ECHO ======================================================================
ECHO.

:: ===== STEP 1: COPY DATABASE DIRECTORIES =====
ECHO [STEP 1] Copying database directories from %vvDataDir% to temp folder...

IF EXIST "%tempBkpDir%" RD /S /Q "%tempBkpDir%"
MD "%tempBkpDir%"
IF ERRORLEVEL 1 (
    SET "errorMsg=Failed to create temp directory: %tempBkpDir%"
    GOTO :SEND_ERROR
)

:: Copy mysql database (required)
IF EXIST "%vvDataDir%\mysql" (
    XCOPY "%vvDataDir%\mysql" "%tempBkpDir%\mysql\" /E /I /Q /Y
    IF ERRORLEVEL 1 GOTO :ERROR_COPY
    ECHO   - mysql [OK]
    SET /A "dbCopyCount+=1"
) ELSE (
    SET "errorMsg=Required directory not found: %vvDataDir%\mysql"
    GOTO :SEND_ERROR
)

:: Copy vvektor database (required)
IF EXIST "%vvDataDir%\%vvektorDB%" (
    XCOPY "%vvDataDir%\%vvektorDB%" "%tempBkpDir%\%vvektorDB%\" /E /I /Q /Y
    IF ERRORLEVEL 1 GOTO :ERROR_COPY
    ECHO   - %vvektorDB% [OK]
    SET /A "dbCopyCount+=1"
) ELSE (
    SET "errorMsg=Required directory not found: %vvDataDir%\%vvektorDB%"
    GOTO :SEND_ERROR
)

:: Copy all databases for the specified year
IF /I "%vvDomena%"=="NULL" (
    SET "yearPattern=vv%vvYear%_*"
) ELSE (
    SET "yearPattern=%vvDomena%_vv%vvYear%_*"
)

SET "yearDbCount=0"
FOR /D %%d IN ("%vvDataDir%\%yearPattern%") DO (
    SET "dbName=%%~nxd"
    XCOPY "%%d" "%tempBkpDir%\!dbName!\" /E /I /Q /Y
    IF ERRORLEVEL 1 GOTO :ERROR_COPY
    ECHO   - !dbName! [OK]
    SET /A "dbCopyCount+=1"
    SET /A "yearDbCount+=1"
)

:: Verify at least one year database was found
IF !yearDbCount! EQU 0 (
    SET "errorMsg=No databases found matching pattern: %yearPattern%"
    GOTO :SEND_ERROR
)

:: Get uncompressed size
FOR /f %%i IN ('powershell -NoProfile -Command "(Get-ChildItem -Path '%tempBkpDir%' -Recurse | Measure-Object -Property Length -Sum).Sum"') DO SET "sizeBeforeBytes=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "[math]::Round((Get-ChildItem -Path '%tempBkpDir%' -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB, 2)"') DO SET "sizeBeforeMB=%%i"

ECHO [STEP 1] Complete. Copied !dbCopyCount! database(s).
ECHO   Uncompressed size: %sizeBeforeMB% MB
ECHO.

:: ===== STEP 2: CREATE COMPRESSED ARCHIVE (7-ZIP) =====
ECHO [STEP 2] Creating compressed archive with 7-Zip (maximum compression)...

:: Delete existing archive file if present
IF EXIST "%archiveFile%" DEL "%archiveFile%"

"%sevenZip%" a -t7z -mx=9 -mfb=64 -md=32m "%archiveFile%" "%tempBkpDir%\*"
SET "compressExitCode=!ERRORLEVEL!"

IF !compressExitCode! NEQ 0 (
    SET "errorMsg=7-Zip compression failed with exit code !compressExitCode!"
    GOTO :SEND_ERROR
)

IF NOT EXIST "%archiveFile%" GOTO :ERROR_COMPRESS

:: Get compressed size and calculate ratio
FOR /f %%i IN ('powershell -NoProfile -Command "(Get-Item '%archiveFile%').Length"') DO SET "sizeAfterBytes=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "[math]::Round((Get-Item '%archiveFile%').Length / 1MB, 2)"') DO SET "sizeAfterMB=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "if (%sizeAfterBytes% -gt 0) { [math]::Round(%sizeBeforeBytes% / %sizeAfterBytes%, 2) } else { 0 }"') DO SET "compressionRatio=%%i"
FOR /f %%i IN ('powershell -NoProfile -Command "if (%sizeBeforeBytes% -gt 0) { [math]::Round(%sizeAfterBytes% / %sizeBeforeBytes% * 100, 1) } else { 0 }"') DO SET "percentOfOriginal=%%i"

:: Cleanup temp directory
RD /S /Q "%tempBkpDir%"

ECHO [STEP 2] Complete: %archiveFile%
ECHO   Compressed size: %sizeAfterMB% MB
ECHO   Compression: %compressionRatio%:1 (%percentOfOriginal%%% of original)
ECHO.

:: ===== STEP 3: ENCRYPT WITH DLOCK2 =====
ECHO [STEP 3] Encrypting backup file...

:: Delete existing encrypted file if present
IF EXIST "%encryptedFile%" DEL "%encryptedFile%"

dlock2 /E /S "%archiveFile%" "%encryptedFile%" /P"%vvPassword%"
SET "dlockExitCode=!ERRORLEVEL!"

IF !dlockExitCode! NEQ 0 (
    SET "errorMsg=dlock2 encryption failed with exit code !dlockExitCode!"
    GOTO :SEND_ERROR
)

IF NOT EXIST "%encryptedFile%" GOTO :ERROR_ENCRYPT

:: Delete unencrypted file
DEL "%archiveFile%"

:: Get final encrypted file size
FOR /f %%i IN ('powershell -NoProfile -Command "[math]::Round((Get-Item '%encryptedFile%').Length / 1MB, 2)"') DO SET "sizeFinalMB=%%i"

ECHO [STEP 3] Complete: %encryptedFile%
ECHO   Final size: %sizeFinalMB% MB
ECHO.

:: ===== STEP 4: DELETE OLD BACKUPS IN BKPDIR =====
ECHO [STEP 4] Cleaning old backups in %vvBkpDir%...

:: Create a temporary PowerShell script to handle cleanup logic
SET "cleanupScript=%TEMP%\vvBkpCleanup.ps1"

(
    ECHO $bkpDir = '%vvBkpDir%'
    ECHO $monday = ^(Get-Date^).AddDays(-^(^(Get-Date^).DayOfWeek.value__ - 1^)^).Date
    ECHO.
    ECHO Get-ChildItem -Path $bkpDir -Filter "vvBkp_*.7z.vvv" ^| ForEach-Object {
    ECHO     # Extract date from filename: vvBkp_XXX_YYYY_YYYYMMDD.7z.vvv
    ECHO     $name = $_.Name -replace '\.7z\.vvv$', ''
    ECHO     $dateStr = $name.Substring^($name.Length - 8^)
    ECHO     try {
    ECHO         $fileDate = [datetime]::ParseExact^($dateStr, 'yyyyMMdd', $null^)
    ECHO         if ^($fileDate -lt $monday -and $fileDate -ne $today^) {
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

ECHO [STEP 4] Complete.
ECHO.

:: ===== STEP 5: COPY TO DROPBOX =====
ECHO [STEP 5] Copying to Dropbox: %vvDropBox%...

IF NOT EXIST "%vvDropBox%" (
    SET "errorMsg=Dropbox directory does not exist: %vvDropBox%"
    GOTO :SEND_ERROR
)

XCOPY "%encryptedFile%" "%vvDropBox%\" /V /Y /F
IF ERRORLEVEL 1 (
    SET "errorMsg=XCOPY to Dropbox failed"
    GOTO :SEND_ERROR
)

SET "copiedBkpFile=%vvDropBox%\%bkpFileName%.7z.%qcFextension%"

IF NOT EXIST "%copiedBkpFile%" GOTO :ERROR_DROPBOX_COPY

ECHO [STEP 5] Complete.
ECHO.

:: ===== STEP 6: DELETE OLD BACKUPS IN DROPBOX =====
ECHO [STEP 6] Cleaning old backups in Dropbox (keeping last 2)...

:: Count and delete old files, keeping only the 2 most recent
SET "fileCount=0"
FOR /f "delims=" %%f IN ('DIR /B /O-D "%vvDropBox%\vvBkp_*.7z.%qcFextension%" 2^>nul') DO (
    SET /A "fileCount+=1"
    IF !fileCount! GTR 2 (
        ECHO   Deleting: %%f
        DEL "%vvDropBox%\%%f"
    ) ELSE (
        ECHO   Keeping: %%f
    )
)

ECHO [STEP 6] Complete.
ECHO.

:: ===== STEP 7: APPEND TO LOG FILE =====
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "endTime=%%i"

:: Calculate duration using PowerShell with proper time parsing
FOR /f %%i IN ('powershell -NoProfile -Command "$s=[timespan]::Parse('%startTime%'); $e=[timespan]::Parse('%endTime%'); ($e - $s).ToString('hh\:mm\:ss')"') DO SET "duration=%%i"

:: Fallback if duration calculation failed
IF "%duration%"=="" SET "duration=00:00:00"
IF "%duration%"=="hh:mm:ss" SET "duration=ERROR"

ECHO [STEP 7] Writing to log file...
ECHO %theYYYYMMDD%	%bkpFileName%	Start: %startTime%	End: %endTime%	Duration: %duration%	Size: %sizeBeforeMB%MB-^>%sizeAfterMB%MB (%percentOfOriginal%%%)	OK >> "%logFile%"

ECHO.
ECHO ======================================================================
ECHO   BACKUP COMPLETED SUCCESSFULLY!
ECHO   File: %bkpFileName%.7z.%qcFextension%
ECHO   Duration: %duration%
ECHO   Size: %sizeBeforeMB% MB -^> %sizeFinalMB% MB (%percentOfOriginal%%% of original)
ECHO ======================================================================
ECHO.

GOTO :END

:: ===== ERROR HANDLERS =====
:ERROR_COPY
SET "errorMsg=Error copying database directories"
GOTO :SEND_ERROR

:ERROR_COMPRESS
SET "errorMsg=Error creating 7z archive: %archiveFile%"
GOTO :SEND_ERROR

:ERROR_ENCRYPT
SET "errorMsg=Error encrypting file: %encryptedFile%"
GOTO :SEND_ERROR

:ERROR_DROPBOX_COPY
SET "errorMsg=Error copying to Dropbox: %copiedBkpFile%"
GOTO :SEND_ERROR

:SEND_ERROR
ECHO.
ECHO ======================================================================
ECHO   BACKUP FAILED!
ECHO   ERROR: %errorMsg%
ECHO ======================================================================
ECHO.

:: Cleanup temp directory if it exists
IF EXIST "%tempBkpDir%" RD /S /Q "%tempBkpDir%"
:: Cleanup partial archive file if it exists
IF EXIST "%archiveFile%" DEL "%archiveFile%"

:: Get end time for logging
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "endTime=%%i"

:: Write error to log file
IF "%bkpFileName%"=="" SET "bkpFileName=UNKNOWN"
ECHO %theYYYYMMDD%	%bkpFileName%	Start: %startTime%	End: %endTime%	ERROR: %errorMsg% >> "%logFile%"

:: Send email notification
blat -subject "[vvBkp ERROR] %vvBkpID%: %errorMsg%" -body "Backup: %bkpFileName%\nError: %errorMsg%\nTime: %DATE% %endTime%" -server %outgoingMailServer% -f %fromEmail% -to %toEmail%

:: Exit with error code
EXIT /B 1

:USAGE
ECHO.
ECHO Usage: vvBkpScript.bat BkpID VvDomena DataDir BkpDir DropBox Year Password
ECHO.
ECHO Example: vvBkpScript.bat ROZEL NULL D:\VIPER\MyVvData D:\VIPER\MyVvBackup D:\VIPER\Dropbox 2026 myPassword
ECHO.
ECHO Requires: 7-Zip installed at "C:\Program Files\7-Zip\7z.exe"
ECHO.
EXIT /B 1

:END
ENDLOCAL
EXIT /B 0