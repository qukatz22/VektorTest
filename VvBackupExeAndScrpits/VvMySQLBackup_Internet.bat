@ECHO off

CLS

::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------

set projName=LAJNUS

set maxmailsize=10000000

SET backupdir=C:\VIPER\MyVvBackup

SET qcFextension=vvv

SET outgoingMailServer=mail.htnet.hr
::SET outgoingMailServer=smtp.gmail.com

SET fromEmail=%projName%@zg.htnet.hr
::SET fromEmail=roberman007@gmail.com

SET toEmailWattachment=roberman007@gmail.com
SET toEmail=viper@zg.htnet.hr

::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------

::====================================================================== 
FOR /f %%i IN ('doff.exe yyyymmdd') DO SET theYYYYMMDD=%%i

SET todayFiles=%backupdir%\*%theYYYYMMDD%*.sql.gz.%qcFextension%
ECHO.
::====================================================================== 
IF EXIST %todayFiles% (
ECHO todayFiles: [%todayFiles%] EXIST.
ECHO.
) ELSE (
ECHO todayFiles: [%todayFiles%] MISSING!
GOTO ERROR1
)
::====================================================================== 
FOR /f "delims=" %%x IN ('DIR /od /a-d /b %todayFiles%') DO SET bkpFileName=%%x
SET recent=%backupdir%\%bkpFileName%
IF EXIST "%recent%" (
ECHO File to be mailed: [%recent%] EXIST.
ECHO.
) ELSE (
ECHO recent: [%recent%] MISSING!
GOTO ERROR2
)
::======================================================================
CALL :MAILFILE "%recent%"
ECHO Backup completed
GOTO :END
::======================================================================
:MAILFILE

IF /i %~z1 LSS %maxmailsize% (
   ECHO Emailing backup file...
   blat -u roberman007@gmail.com -pw iterasupo -q -attach %1 -subject "BkpFile: [%recent%]" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmailWattachment%
) ELSE (
   ECHO Size of backup file %~z1 B exceeds configured email size %maxmailsize% B!
   GOTO ERROR3
)
GOTO :END
::======================================================================
:ERROR1
blat -subject "todayFiles: [%todayFiles%] MISSING!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:ERROR2
blat -subject "recent file: [%recent%] MISSING!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:ERROR3
blat -subject "FileSize [%~z1 B] of %BkpFfile% [%recent%] TOO BIG!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================

:END
