@ECHO off
:: ======================================================================
:: vvRestore_CLD.bat - MySQL Database Restore Script
:: Usage: vvRestore_CLD.bat <encryptedFile.tar.gz.vvv> <targetDir> <password>
:: Example: vvRestore_CLD.bat "D:\Backup\vvBkp_ROZEL_2026_20260325.tar.gz.vvv" "D:\VIPER\MyVvData" "trtmrkzvrK^01"
:: ======================================================================

SET "encryptedFile=%~1"
SET "targetDir=%~2"
SET "password=%~3"

:: Validate parameters
IF "%encryptedFile%"=="" GOTO :USAGE
IF "%targetDir%"=="" GOTO :USAGE
IF "%password%"=="" GOTO :USAGE

:: Get tar.gz filename (remove only .vvv extension)
:: %~dpn1 removes last extension, so file.tar.gz.vvv becomes file.tar.gz
SET "tarFile=%~dpn1"

ECHO.
ECHO ======================================================================
ECHO   vvRestore - Starting restore
ECHO   Source: %encryptedFile%
ECHO   Target: %targetDir%
ECHO ======================================================================
ECHO.

:: Decrypt
ECHO [STEP 1] Decrypting...
dlock2 /D /S "%encryptedFile%" "%tarFile%" /P"%password%"

IF NOT EXIST "%tarFile%" (
    ECHO Error: Decryption failed!
    EXIT /B 1
)
ECHO [STEP 1] Complete: %tarFile%
ECHO.

:: Extract
ECHO [STEP 2] Extracting to %targetDir%...
tar -xzvf "%tarFile%" -C "%targetDir%"

IF ERRORLEVEL 1 (
    ECHO Error: Extraction failed!
    EXIT /B 1
)
ECHO [STEP 2] Complete.
ECHO.

:: Cleanup
DEL "%tarFile%"

ECHO ======================================================================
ECHO   RESTORE COMPLETED SUCCESSFULLY!
ECHO   Databases restored to: %targetDir%
ECHO ======================================================================

GOTO :END

:USAGE
ECHO.
ECHO Usage: vvRestore_CLD.bat ^<encryptedFile.tar.gz.vvv^> ^<targetDir^> ^<password^>
ECHO.
ECHO Example: vvRestore_CLD.bat "D:\Backup\vvBkp_ROZEL_2026_20260325.tar.gz.vvv" "D:\VIPER\MyVvData" "myPassword"
ECHO.

:END