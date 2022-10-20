
Ovo je samo proba!!!

@ECHO off

CLS

::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------

set projName=VEKTOR

SET sourcedir=D:\000_QCS\Vektor
SET backupRootDir=E:\VIPER_VEKTOR_BACKUPS

::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------

::====================================================================== 
FOR /f %%i IN ('doff.exe yyyymmdd') DO SET theYYYYMMDD=%%i

SET todayBackupDir=%backupRootDir%\Vv%theYYYYMMDD%

::====================================================================== 
XCOPY %sourcedir% %todayBackupDir% /Y /S /E /V /I

gzip %todayBackupDir%
