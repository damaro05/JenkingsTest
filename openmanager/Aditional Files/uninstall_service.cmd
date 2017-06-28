JBCHostSrv.exe -uninstall
echo off
if ERRORLEVEL 1 (
ECHO -uninstall error 
pause
) else ( 
IF [%1]==[] pause
)