@ECHO off

CLS

::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------

set projName=VEKTOR

SET sourceDir1=D:\000_QCS\Vektor
SET sourceDir2=D:\000_QCS\Surger
SET sourceDir3=D:\000_QCS\Remonster
SET sourceDir4=D:\000_QCS\SkyLab

SET backupNamePreffix=VvSrcBkp
SET backupRootDir=E:\VIPER_VEKTOR_BACKUPS
:SET backupRootDir=D:\VIPER_VEKTOR_BACKUPS
SET toEmail=roberman007@gmail.com
SET theWord=nuix84

:SET  backupTaker1=PERO
SET  backupTaker1=VVKRISTAL
SET  bkpTakerDir1=VektorBackups
SET bkpTakerPath1=\\%backupTaker1%\%bkpTakerDir1%

SET bkpDropboxDir=E:\Dropbox\VektorBackup
:SET bkpDropboxDir=D:\Dropbox\VektorBackup

set maxmailsize=32000000
SET outgoingMailServer=mail.htnet.hr
SET fromEmail=%projName%@zg.htnet.hr
SET toEmailWattachment=roberman007@gmail.com
SET toEmail=viper@zg.htnet.hr

::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------

::====================================================================

"d:\Program Files\WinRAR\rar" a -r -y -ag_YYYYMMDD -hp%theWord% -p%theWord% %backupRootDir%\%backupNamePreffix% %sourceDir1% %sourceDir2% %sourceDir3% %sourceDir4%

::"d:\Program Files\WinRAR\rar" a -r -y -ag_YYYYMMDD -v11000 -hp%theWord% -p%theWord% -ieml%toEmailWattachment% %backupRootDir%\%backupNamePreffix% %sourceDir1% %sourceDir2% %sourceDir3%
::rar a -r -y -ag_YYYYMMDD -hp%theWord% -p%theWord% -v8192 %backupRootDir%\%backupNamePreffix% %sourceDir1% %sourceDir2% %sourceDir3%

FOR /f %%i IN ('doff.exe yyyymmdd') DO SET theYYYYMMDD=%%i

SET backupFullFilePathAndName=%backupRootDir%\%backupNamePreffix%_%theYYYYMMDD%*.rar

XCOPY "%backupFullFilePathAndName%" "%bkpTakerPath1%" /V /Y /F

XCOPY "%backupFullFilePathAndName%" "%bkpDropboxDir%" /V /Y /F

PAUSE






::======================================================================
::ovu dole liniju odremarkiraj kada oces sos blat-om
::CALL :MAILFILE "%backupFullFilePathAndName%"
ECHO Backup completed
GOTO :END
::======================================================================
:MAILFILE

IF /i %~z1 LSS %maxmailsize% (
   ECHO Emailing backup file...
   blat -q -attach %1 -subject "BkpFile: [%backupFullFilePathAndName%]" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmailWattachment%
) ELSE (
   ECHO Size of backup file %~z1 B exceeds configured email size %maxmailsize% B!
   GOTO ERROR3
)
GOTO :END
::======================================================================
:ERROR3
blat -subject "FileSize [%~z1 B] of %BkpFfile% [%recent%] TOO BIG!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================

:END
