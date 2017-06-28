JBCHostSrv.exe -install
echo off
if ERRORLEVEL 1 (
ECHO -install error 
pause
) else ( 
IF [%1]==[] pause
)