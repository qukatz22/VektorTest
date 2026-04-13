@ECHO off
SETLOCAL EnableDelayedExpansion

:: ======================================================================
:: vvRestore.bat - MySQL Database Restore Script
:: Usage: vvRestore.bat <encryptedFile.7z.vvv> <targetDir> <password>
:: Example: vvRestore.bat "D:\Backup\vvBkp_ROZEL_2026_20260325.7z.vvv" "D:\VIPER\MyVvData" "myPassword"
:: Requires: 7-Zip installed at "C:\Program Files\7-Zip\7z.exe"
:: ======================================================================

SET "encryptedFile=%~1"
SET "targetDir=%~2"
SET "password=%~3"

:: Validate parameters
IF "%encryptedFile%"=="" GOTO :USAGE
IF "%targetDir%"=="" GOTO :USAGE
IF "%password%"=="" GOTO :USAGE

:: ===== CHECK FOR 7-ZIP =====
SET "sevenZip="
IF EXIST "C:\Program Files\7-Zip\7z.exe" SET "sevenZip=C:\Program Files\7-Zip\7z.exe"
IF EXIST "C:\Program Files (x86)\7-Zip\7z.exe" SET "sevenZip=C:\Program Files (x86)\7-Zip\7z.exe"

IF NOT DEFINED sevenZip (
    ECHO.
    ECHO ERROR: 7-Zip not installed. Please install from https://www.7-zip.org/
    EXIT /B 1
)

:: Validate encrypted file exists
IF NOT EXIST "%encryptedFile%" (
    ECHO.
    ECHO ERROR: Encrypted file not found: %encryptedFile%
    EXIT /B 1
)

:: Validate target directory exists
IF NOT EXIST "%targetDir%" (
    ECHO.
    ECHO ERROR: Target directory does not exist: %targetDir%
    EXIT /B 1
)

:: Get archive filename (remove only .vvv extension)
:: %~dpn1 removes last extension, so file.7z.vvv becomes file.7z
SET "archiveFile=%~dpn1"

ECHO.
ECHO ======================================================================
ECHO   vvRestore - Starting restore
ECHO   Source: %encryptedFile%
ECHO   Target: %targetDir%
ECHO ======================================================================
ECHO.

:: ===== STEP 1: DECRYPT =====
ECHO [STEP 1] Decrypting...

:: Delete existing archive file if present
IF EXIST "%archiveFile%" DEL "%archiveFile%"

dlock2 /D /S "%encryptedFile%" "%archiveFile%" /P"%password%"
SET "dlockExitCode=!ERRORLEVEL!"

IF !dlockExitCode! NEQ 0 (
    ECHO ERROR: dlock2 decryption failed with exit code !dlockExitCode!
    EXIT /B 1
)

IF NOT EXIST "%archiveFile%" (
    ECHO ERROR: Decryption failed - output file not created!
    EXIT /B 1
)

ECHO [STEP 1] Complete: %archiveFile%
ECHO.

:: ===== STEP 2: EXTRACT WITH 7-ZIP =====
ECHO [STEP 2] Extracting to %targetDir%...

"%sevenZip%" x "%archiveFile%" -o"%targetDir%" -y
SET "extractExitCode=!ERRORLEVEL!"

IF !extractExitCode! NEQ 0 (
    ECHO ERROR: 7-Zip extraction failed with exit code !extractExitCode!
    DEL "%archiveFile%"
    EXIT /B 1
)

ECHO [STEP 2] Complete.
ECHO.

:: ===== STEP 3: CLEANUP =====
ECHO [STEP 3] Cleaning up temporary files...
DEL "%archiveFile%"
ECHO [STEP 3] Complete.
ECHO.

ECHO ======================================================================
ECHO   RESTORE COMPLETED SUCCESSFULLY!
ECHO   Databases restored to: %targetDir%
ECHO ======================================================================

GOTO :END

:USAGE
ECHO.
ECHO Usage: vvRestore.bat ^<encryptedFile.7z.vvv^> ^<targetDir^> ^<password^>
ECHO.
ECHO Example: vvRestore.bat "D:\Backup\vvBkp_ROZEL_2026_20260325.7z.vvv" "D:\VIPER\MyVvData" "myPassword"
ECHO.
ECHO Requires: 7-Zip installed at "C:\Program Files\7-Zip\7z.exe"
ECHO.
EXIT /B 1

:END
ENDLOCAL
EXIT /B 0