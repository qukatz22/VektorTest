@ECHO off
SETLOCAL EnableDelayedExpansion

:: --- PARAMETRI IZ KOMANDNE LINIJE ---
SET vvBkpID=%1
SET vvDomena=%2
SET vvDataDir=%3
SET vvBkpDir=%4
SET vvDropBox=%5
SET vvYear=%6
SET vvPassword=%7

:: --- INTERNE POSTAVKE ---
SET "startTime=%time%"
FOR /F "tokens=2-4 delims=/ " %%a IN ('date /t') DO SET today=%%c%%a%%b
FOR /F "tokens=1-3 delims=:." %%a IN ("%time%") DO SET tstamp=%%a%%b%%c
SET start_s=!time!

:: --- DEFINICIJA NAZIVA FAJLA ---
IF /I "%vvDomena%"=="NULL" (
    SET bkpFileName=vvBkp_%vvBkpID%_%vvYear%_%today%
    SET vvektorDB=vvektor
) ELSE (
    SET bkpFileName=vvBkp_%vvDomena%_%vvBkpID%_%vvYear%_%today%
    SET vvektorDB=%vvDomena%_vvektor
)

SET tempWorkDir=%vvBkpDir%\%bkpFileName%_temp
SET finalArchive=%vvBkpDir%\%bkpFileName%.sql.gz
SET encodedFile=%finalArchive%.vvv

:: --- 1. KOPIRANJE BAZA (FILE LEVEL) ---
IF NOT EXIST "%tempWorkDir%" MKDIR "%tempWorkDir%"

:: Kopiraj mysql i vvektor (uvijek)
XCOPY "%vvDataDir%\mysql" "%tempWorkDir%\mysql\" /S /E /I /Y /Q
XCOPY "%vvDataDir%\%vvektorDB%" "%tempWorkDir%\%vvektorDB%\" /S /E /I /Y /Q

:: Kopiraj baze koje sadrže godinu (vvYear)
IF /I "%vvDomena%"=="NULL" (
    SET searchPattern=vv%vvYear%_*
) ELSE (
    SET searchPattern=%vvDomena%_vv%vvYear%_*
)

FOR /D %%D IN ("%vvDataDir%\%searchPattern%") DO (
    XCOPY "%%D" "%tempWorkDir%\%%~nD\" /S /E /I /Y /Q
)

:: --- 2. GZIP (Cijeli temp folder u jedan fajl) ---
:: Napomena: Gzip obično radi s pojedinačnim fajlovima. 
:: Preporuka: Koristite 'tar' (ugrađen u Win10+) pa onda gzip ili samo 7zip.
tar -czf "%finalArchive%" -C "%tempWorkDir%" .
IF ERRORLEVEL 1 GOTO MAILERROR

:: --- 3. ENCRYPT (dlock2) ---
dlock2 /E /S "%finalArchive%" "%encodedFile%" /P"%vvPassword%"
IF NOT EXIST "%encodedFile%" GOTO MAILERROR
DEL "%finalArchive%"
RD /S /Q "%tempWorkDir%"

:: --- 4. BRISANJE STARIH BACKUPOVA U BKPDIR ---
:: Čuva sve iz tekućeg tjedna, od starijih samo petak (Day 5)
forfiles /p "%vvBkpDir%" /m "*.vvv" /d -7 /c "cmd /c powershell -Command \"if ((Get-Item @path).CreationTime.DayOfWeek -ne 'Friday') { Remove-Item @path }\""

:: --- 5. COPY TO DROPBOX ---
XCOPY "%encodedFile%" "%vvDropBox%\" /Y /V
IF ERRORLEVEL 1 GOTO MAILERROR

:: --- 6. ROTACIJA U DROPBOXU (Ostavi zadnja dva) ---
FOR /F "skip=2 delims=" %%F IN ('DIR "%vvDropBox%\*.vvv" /B /O-D') DO DEL "%vvDropBox%\%%F"

:: --- 7. LOGIRANJE ---
SET "endTime=%time%"
ECHO %date% %startTime% - %endTime% : %bkpFileName% >> "%vvBkpDir%\backup_log.txt"

GOTO END

:MAILERROR
:: Ovdje ubaci svoj blat ili SwithMail komandu
ECHO Greška u backupu!
GOTO END

:END
