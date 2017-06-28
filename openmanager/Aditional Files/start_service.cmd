JBCHostSrv.exe -start
echo off
if ERRORLEVEL 1 (
ECHO -start error 
pause
) else ( 
IF [%1]==[] pause
)