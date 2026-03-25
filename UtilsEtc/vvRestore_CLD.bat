@ECHO off
:: usage: vvRestore_CLD.bat <encryptedFile.vvv> <targetDir> <password>
:: Example: vvRestore_CLD.bat "D:\Backup\vvBkp_ROZEL_2026_20260325.vvv" "D:\VIPER\MyVvData" "trtmrkzvrK^01"

SET "encryptedFile=%1"
SET "targetDir=%2"
SET "password=%3"

:: Get base filename without .vvv extension
SET "tarFile=%~dpn1.tar.gz"

:: Decrypt
dlock2 /D /S "%encryptedFile%" "%tarFile%" /P"%password%"

IF NOT EXIST "%tarFile%" (
    ECHO Error: Decryption failed!
    EXIT /B 1
)

:: Extract
tar -xzvf "%tarFile%" -C "%targetDir%"

:: Cleanup
DEL "%tarFile%"

ECHO Restore complete to: %targetDir%