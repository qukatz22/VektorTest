## Overview of task:

Imam backup skriptu MySql baza na windowsima, koja se je pokretala nakon što
bi backup izradio Mysql Administrator preko Taskschedulera.
S vremenom, taj način je postao neadekvatan, jer je zbirna veličina svih baza
koje treba backupirati narasla na preko 1GB te Mysql Administrator backup traje cca 12 sati.
Na dnu ove poruke se nalazi sadašnja skripta tebi za analizu.

Skripta ide na više produkcijskih lokacija, neke imaju neke nemaju `%vvDomena%`
Ukoliko se pojavi problem s izvođenjem pojedinog segmenta skripte potrebno je 
poslati email poruku supportu. Parametre za mail client ćeš naći u skripti primjera.
Ima li bolji program od blat-a?
Na kraju backupa treba pobrisati stare backupove u `%vvBkpDir%`, 
stari su svi koji nisu iz tekućeg tjedna,
a od prethodnih tjedana ostaviti samo bkp-ove od petka.
Nakon toga treba iskopirati upravo izrađeni bkp u `%vvDropBox%`directory 
a u tom `%vvDropBox%`dir. pobrisati stare backupove, ostaviti samo zadnji i predzadnji.
Trebam i kreiranje log file-a u `%vvBkpDir%`, svaki izvedeni backup da kreira redak sa informacijama
vrijeme početka, vrijeme završetka, trajanje.

## Dijagam toka:

1. copy all wanted database directoryes from `%vvDataDir%`to `%vvBkpDir%`
2. gzip it all in one single file 
3. encrypt the file using dlock2 program
4. delete old backups in `%vvBkpDir%`
5. makecopy of that file in `%vvDropBox%` 
6. delete old backups in `%vvDropBox%`
7. append new line in log file

## Parametri komandne linije
 
1. `%vvBkpID%`    = string representing name of backup
2. `%vvDomena%`   = string representing short name of project
3. `%vvDataDir%`  = database directory (source of backup on local disk)
4. `%vvBkpDir%`   = backup   directory (target of backup on local disk)
5. `%vvDropBox%`  = Dropbox  directory (target of cloud version of backup)
6. `%vvYear%`     = project year of databases
7. `%vvPassword%` = passphrase for dlock2
 
## Primjer komandne linije upotrebe skripte:
 
vvBkpScript ROZEL NULL D:\VIPER\MyVvData D:\VIPER\MyVvBackup D:\VIPER\Dropbox 2026 trtmrkzvrK^01
            1.    2.   3.                4.                  5.               6.   7.

## Kako će se zvati bkp fajl kada ima `%vvDomena%`(nije NULL):

vvBkp_vvDU_DUCATI_2026_20260324

vvBkp_   = literal
vvDU_    = `%vvDomena%`_
DUCATI_  = `%vvBkpID%`_
2026_    = `%vvYear%`_
20260324 = YYYYMMDD (date of backup execution)

## Kako će se zvati bkp fajl kada nema `%vvDomena%`(je NULL):

vvBkp_ROZEL_2026_20260324

vvBkp_   = literal
ROZEL_   = `%vvBkpID%`_
2026_    = `%vvYear%`_
20260324 = YYYYMMDD (date of backup execution)
   
 1. Baze koje treba backupirati:
 sve zatečeno u `%vvBkpDir%`direktoriju što se odnosi na `%vvYear%`godinu      
 plus 'mysql' bazu, plus 'vvektor' bazu (`%vvDomena%`_vvektor ako ima `%vvDomena%`)  
 
## Primjeri naziva database-a kada ima `%vvDomena%`(nije NULL):

mysql        - uvijek bez obzira na godinu
vvBR_vvektor - uvijek bez obzira na godinu
vvDU_vv2026_DUCATI_000016
vvDE_vv2018_VERIDI_000044

## Primjeri naziva database-a kada nema `%vvDomena%`(je NULL):
 
mysql   - uvijek bez obzira na godinu
vvektor - uvijek bez obzira na godinu
vv2023_ROZUSL_003180
vv2026_ROZUSL_003180

## Naming convention kada ima `%vvDomena%`(na primjeru 'vvDU_vv2026_DUCATI_000016'):

'vvDU_'   = `%vvDomena%`_
'vv2026_' = vv`%vvYear%`_
'DUCATI_' = string representing ticker of project plus "_"
'000016'  = int as string representing numID of project

## Naming convention kada nema `%vvDomena%`(na primjeru 'vv2026_ROZUSL_003180'):

'vv2026_' = vv`%vvYear%`_
'ROZUSL_' = string representing ticker of project plus "_"
'003180'  = int as string representing numID of project

Ovo je stara skripta:

@ECHO off

CLS

::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------
::------------ DOLE DIRAJ ---------------------

set projName=LAJNUS

SET backupdir=C:\VIPER\MyVvBackup
SET backupdirDB=C:\VIPER\Dropbox

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
IF EXIST %todayFiles%(
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
:: obrisi starije od 7 dana iz DropBox directorya
forfiles /p "%backupdirDB%" /s /m "*.%qcFextension%" /c "cmd /c Del @path" /d -7
XCOPY "%encoded%" "%backupdirDB%" /V /Y /F
SET copiedBkpFile=%backupdirDB%\%bkpFileName%.%qcFextension%
IF EXIST "%copiedBkpFile%" (
ECHO Copied bkpFile for Dropbox: [%copiedBkpFile%] EXIST.
ECHO.
) ELSE (
ECHO Copied bkpFile for Dropbox: [%copiedBkpFile%] MISSING!
GOTO MAILERROR5
)
::======================================================================

ECHO.
ECHO ALLES IS OK!
ECHO.

GOTO END

::======================================================================
:MAILERROR1
blat -subject "todayFiles: [%todayFiles%] MISSING!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
GOTO END
::======================================================================
:MAILERROR2
blat -subject "recent file: [%recent%] MISSING!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
GOTO END
::======================================================================
:MAILERROR6
blat -subject "Encoded: [%encoded%] MISSING!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
GOTO END
::======================================================================
:MAILERROR3
blat -subject "Ping to [%backupTaker1%] FAILED!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
GOTO END
::======================================================================
:MAILERROR4
blat -subject "bkpTakerPath1: [%bkpTakerPath1%] NOT REACHABLE!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
GOTO END
::======================================================================
:MAILERROR5
blat -subject "copiedBkpFile: [%copiedBkpFile%] MISSING!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
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
blat -subject "Deletion of: [%recent%] FAILED!" -body " " -server %outgoingMailServer%-f %fromEmail%-to %toEmail%
GOTO END
::======================================================================

:END
