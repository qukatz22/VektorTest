@echo off

SET theDatePattern=dd.mm.yy
SET ftpServer=192.168.0.26
::SET ftpServer=172.17.2.26
SET userName=root
SET password=romaco
SET backupDir=/usr/qukatz/q_day_bkp

SET tmpFile=theTmpFile.txt

::------------ DOLE NE DIRAJ ---------------------

FOR /f %%i IN ('doff.exe %theDatePattern%') DO SET theDate=%%i

ECHO open %ftpServer%   >  %tmpFile%
ECHO %userName%>> %tmpFile%
ECHO %password%>> %tmpFile%
ECHO cd %backupDir%     >> %tmpFile%
ECHO binary             >> %tmpFile%
ECHO mget *%theDate%*   >> %tmpFile%
ECHO bye                >> %tmpFile%

FTP -i -s:%tmpFile% 

