@echo off

SET theDatePattern=yyyy_mm_dd
SET ftpServer=192.168.0.26
::SET ftpServer=172.17.2.26
SET userName=root
SET password=romaco
SET localDir=C:\off_izvodi
SET remoteDir=/usr/qukatz/off_izvodi

SET tmpFile=theTmpFile.txt

::------------ DOLE NE DIRAJ ---------------------

FOR /f %%i IN ('doff.exe %theDatePattern%') DO SET theDate=%%i

SET theFile="izv_%theDate%.dat"

ECHO open %ftpServer%   >  %tmpFile%
ECHO %userName%>> %tmpFile%
ECHO %password%>> %tmpFile%
ECHO cd %localDir% >> %tmpFile%

ECHO put %theFile% %remoteDir%/%theFile% >> %tmpFile%
ECHO bye >> %tmpFile%

ECHO "copying %localDir%\izvod.dat" 
copy "%localDir%\izvod.dat" "izv_%theDate%.dat"

FTP -i -s:%tmpFile% 
