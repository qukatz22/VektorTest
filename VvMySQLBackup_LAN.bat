@ECHO off

CLS

::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------

set projName=JORDAN

SET backupdir=C:\MyVvBackup

SET  backupTaker1=Ripley
SET  bkpTakerDir1=SharedDocs\MyVvBackup
SET bkpTakerPath1=\\%backupTaker1%\%bkpTakerDir1%

SET outgoingMailServer=mail.htnet.hr

SET fromEmail=%projName%@zg.htnet.hr

SET theword=qweqwe
SET qcFextension=vvv

::SET toEmail=roberman007@gmail.com
SET toEmail=viper@zg.htnet.hr

::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------
::------------ DOLE NE DIRAJ ---------------------

::====================================================================== 
gzip %backupdir%\*.sql
::====================================================================== 
FOR /f %%i IN ('doff.exe yyyymmdd') DO SET theYYYYMMDD=%%i

SET todayFiles=%backupdir%\*%theYYYYMMDD%*.sql.gz
ECHO.
::====================================================================== 
IF EXIST %todayFiles% (
ECHO todayFiles: [%todayFiles%] EXIST.
ECHO.
) ELSE (
ECHO todayFiles: [%todayFiles%] MISSING!
GOTO MAILERROR1
)
::====================================================================== 
FOR /f "delims=" %%x IN ('DIR /od /a-d /b %todayFiles%') DO SET bkpFileName=%%x
SET recent=%backupdir%\%bkpFileName%
SET encoded=%recent%.%qcFextension%
IF EXIST "%recent%" (
ECHO File to be copied: [%recent%] EXIST.
ECHO.
) ELSE (
ECHO recent: [%recent%] MISSING!
GOTO MAILERROR2
)
::======================================================================
dlock2 /E /S "%recent%" "%encoded%" /P"%theword%"
IF EXIST "%encoded%" (
DEL "%recent%"
CALL :DELCHECK
ECHO Encoded: [%encoded%] EXIST.
ECHO.
) ELSE (
ECHO Encoded: [%encoded%] MISSING!
GOTO MAILERROR6
)
::======================================================================
ping %backupTaker1% -n 1|find "Reply"

if NOT errorlevel 1 (
SET backupTaker1Alive=YES
ECHO Ping to [%backupTaker1%] OK.
ECHO.
) ELSE (
SET backupTaker1Alive=NO
ECHO Ping to [%backupTaker1%] FAILED!
ECHO.
GOTO MAILERROR3
)
::======================================================================
IF NOT EXIST "%bkpTakerPath1%" MD "%bkpTakerPath1%"

IF EXIST "%bkpTakerPath1%" (
ECHO Copy to path: [%bkpTakerPath1%] EXIST.
ECHO.
) ELSE (
ECHO Copy to path: [%bkpTakerPath1%] MISSING!
GOTO MAILERROR4
)
::======================================================================
XCOPY "%encoded%" "%bkpTakerPath1%" /V /Y /F
SET copiedBkpFile=%bkpTakerPath1%\%bkpFileName%.%qcFextension%
IF EXIST "%copiedBkpFile%" (
ECHO Copied bkpFile: [%copiedBkpFile%] EXIST.
ECHO.
) ELSE (
ECHO Copied bkpFile: [%copiedBkpFile%] MISSING!
GOTO MAILERROR5
)
::======================================================================

ECHO.
ECHO ALLES IS OK!
ECHO.

GOTO END

::======================================================================
:MAILERROR1
blat -subject "todayFiles: [%todayFiles%] MISSING!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:MAILERROR2
blat -subject "recent file: [%recent%] MISSING!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:MAILERROR6
blat -subject "Encoded: [%encoded%] MISSING!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:MAILERROR3
blat -subject "Ping to [%backupTaker1%] FAILED!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:MAILERROR4
blat -subject "bkpTakerPath1: [%bkpTakerPath1%] NOT REACHABLE!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:MAILERROR5
blat -subject "copiedBkpFile: [%copiedBkpFile%] MISSING!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================
:DELCHECK
IF EXIST "%recent%" (
ECHO Couldn't delete: ["%recent%"]!
ECHO.
GOTO MAILERROR7
)
GOTO END
::======================================================================
:MAILERROR7
blat -subject "Deletion of: [%recent%] FAILED!" -body " " -server %outgoingMailServer% -f %fromEmail% -to %toEmail%
GOTO END
::======================================================================

:END
