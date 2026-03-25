@ECHO off
SETLOCAL EnableDelayedExpansion

:: ======================================================================
:: vvBkpScript.bat - MySQL Database Backup Script
:: Usage: vvBkpScript.bat BkpID Domain DataDir BkpDir DropBox Year Password
:: ======================================================================

CLS

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

SET "logFile=%vvBkpDir%\vvBkpLog.txt"
SET "tempBkpDir=%vvBkpDir%\temp_%theYYYYMMDD%"
SET "tarFile=%vvBkpDir%\%bkpFileName%.tar.gz"
SET "encryptedFile=%vvBkpDir%\%bkpFileName%.%qcFextension%"

ECHO.
ECHO ======================================================================
ECHO   vvBkpScript - Starting backup: %bkpFileName%
ECHO   Start time: %startTimestamp%
ECHO ======================================================================
ECHO.

:: ===== STEP 1: COPY DATABASE DIRECTORIES =====
ECHO [STEP 1] Copying database directories from %vvDataDir% to temp folder...

IF EXIST "%tempBkpDir%" RD /S /Q "%tempBkpDir%"
MD "%tempBkpDir%"

:: Copy mysql database (always)
IF EXIST "%vvDataDir%\mysql" (
    XCOPY "%vvDataDir%\mysql" "%tempBkpDir%\mysql\" /E /I /Q /Y
    IF ERRORLEVEL 1 GOTO :ERROR_COPY
    ECHO   - mysql [OK]
) ELSE (
    ECHO   - mysql [NOT FOUND - SKIPPING]
)

:: Copy vvektor database
IF EXIST "%vvDataDir%\%vvektorDB%" (
    XCOPY "%vvDataDir%\%vvektorDB%" "%tempBkpDir%\%vvektorDB%\" /E /I /Q /Y
    IF ERRORLEVEL 1 GOTO :ERROR_COPY
    ECHO   - %vvektorDB% [OK]
) ELSE (
    ECHO   - %vvektorDB% [NOT FOUND - SKIPPING]
)

:: Copy all databases for the specified year
IF /I "%vvDomena%"=="NULL" (
    SET "yearPattern=vv%vvYear%_*"
) ELSE (
    SET "yearPattern=%vvDomena%_vv%vvYear%_*"
)

FOR /D %%d IN ("%vvDataDir%\%yearPattern%") DO (
    SET "dbName=%%~nxd"
    XCOPY "%%d" "%tempBkpDir%\!dbName!\" /E /I /Q /Y
    IF ERRORLEVEL 1 GOTO :ERROR_COPY
    ECHO   - !dbName! [OK]
)

ECHO [STEP 1] Complete.
ECHO.

:: ===== STEP 2: GZIP INTO SINGLE FILE =====
ECHO [STEP 2] Creating compressed archive...

pushd "%tempBkpDir%"
tar -czvf "%tarFile%" *
popd

IF NOT EXIST "%tarFile%" GOTO :ERROR_GZIP

:: Cleanup temp directory
RD /S /Q "%tempBkpDir%"

ECHO [STEP 2] Complete: %tarFile%
ECHO.

:: ===== STEP 3: ENCRYPT WITH DLOCK2 =====
ECHO [STEP 3] Encrypting backup file...

dlock2 /E /S "%tarFile%" "%encryptedFile%" /P"%vvPassword%"

IF NOT EXIST "%encryptedFile%" GOTO :ERROR_ENCRYPT

:: Delete unencrypted file
DEL "%tarFile%"

ECHO [STEP 3] Complete: %encryptedFile%
ECHO.

:: ===== STEP 4: DELETE OLD BACKUPS IN BKPDIR =====
ECHO [STEP 4] Cleaning old backups in %vvBkpDir%...

:: Create a temporary PowerShell script to handle cleanup logic
SET "cleanupScript=%TEMP%\vvBkpCleanup.ps1"

(
    ECHO $bkpDir = '%vvBkpDir%'
    ECHO $ext = '%qcFextension%'
    ECHO $monday = ^(Get-Date^).AddDays(-^(^(Get-Date^).DayOfWeek.value__ - 1^)^).Date
    ECHO.
    ECHO Get-ChildItem -Path $bkpDir -Filter "vvBkp_*.$ext" ^| ForEach-Object {
    ECHO     $dateStr = $_.BaseName.Substring^($_.BaseName.Length - 8^)
    ECHO     $fileDate = [datetime]::ParseExact^($dateStr, 'yyyyMMdd', $null^)
    ECHO     if ^($fileDate -lt $monday^) {
    ECHO         if ^($fileDate.DayOfWeek -ne 'Friday'^) {
    ECHO             Write-Host "  Deleting: $^($_.Name^)"
    ECHO             Remove-Item $_.FullName -Force
    ECHO         } else {
    ECHO             Write-Host "  Keeping ^(Friday^): $^($_.Name^)"
    ECHO         }
    ECHO     } else {
    ECHO         Write-Host "  Keeping ^(current week^): $^($_.Name^)"
    ECHO     }
    ECHO }
) > "%cleanupScript%"

powershell -NoProfile -ExecutionPolicy Bypass -File "%cleanupScript%"
DEL "%cleanupScript%" 2>nul

ECHO [STEP 4] Complete.
ECHO.

:: ===== STEP 5: COPY TO DROPBOX =====
ECHO [STEP 5] Copying to Dropbox: %vvDropBox%...

XCOPY "%encryptedFile%" "%vvDropBox%\" /V /Y /F
SET "copiedBkpFile=%vvDropBox%\%bkpFileName%.%qcFextension%"

IF NOT EXIST "%copiedBkpFile%" GOTO :ERROR_DROPBOX_COPY

ECHO [STEP 5] Complete.
ECHO.

:: ===== STEP 6: DELETE OLD BACKUPS IN DROPBOX =====
ECHO [STEP 6] Cleaning old backups in Dropbox (keeping last 2)...

:: Count and delete old files, keeping only the 2 most recent
SET "fileCount=0"
FOR /f "delims=" %%f IN ('DIR /B /O-D "%vvDropBox%\vvBkp_*.%qcFextension%" 2^>nul') DO (
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
ECHO %theYYYYMMDD%	%bkpFileName%	Start: %startTime%	End: %endTime%	Duration: %duration%	OK >> "%logFile%"

ECHO.
ECHO ======================================================================
ECHO   BACKUP COMPLETED SUCCESSFULLY!
ECHO   File: %bkpFileName%.%qcFextension%
ECHO   Duration: %duration%
ECHO ======================================================================
ECHO.

GOTO :END

:: ===== ERROR HANDLERS =====
:ERROR_COPY
SET "errorMsg=Error copying database directories"
GOTO :SEND_ERROR

:ERROR_GZIP
SET "errorMsg=Error creating gzip archive: %tarFile%"
GOTO :SEND_ERROR

:ERROR_ENCRYPT
SET "errorMsg=Error encrypting file: %encryptedFile%"
GOTO :SEND_ERROR

:ERROR_DROPBOX_COPY
SET "errorMsg=Error copying to Dropbox: %copiedBkpFile%"
GOTO :SEND_ERROR

:SEND_ERROR
ECHO.
ECHO ERROR: %errorMsg%
ECHO.
FOR /f %%i IN ('powershell -NoProfile -Command "Get-Date -Format HH:mm:ss"') DO SET "endTime=%%i"
ECHO %theYYYYMMDD%	%bkpFileName%	Start: %startTime%	End: %endTime%	ERROR: %errorMsg% >> "%logFile%"
blat -subject "[vvBkp ERROR] %vvBkpID%: %errorMsg%" -body "Backup: %bkpFileName%\nError: %errorMsg%\nTime: %DATE% %endTime%" -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO :END

:USAGE
ECHO.
ECHO Usage: vvBkpScript.bat BkpID Domain DataDir BkpDir DropBox Year Password
ECHO.
ECHO Example: vvBkpScript.bat ROZEL NULL D:\VIPER\MyVvData D:\VIPER\MyVvBackup D:\VIPER\Dropbox 2026 myPassword
ECHO.
GOTO :END

:END
ENDLOCAL