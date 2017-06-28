JBCHostSrv.exe -stop
echo off
if ERRORLEVEL 1 (
ECHO -stop error 
pause
) else ( 
IF [%1]==[] pause
)