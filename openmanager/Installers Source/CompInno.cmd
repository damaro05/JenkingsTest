rem @echo off
rem Añade nueva compilación al principio del log
set compname=%1
set tempname=%1_old

ren %compname%.log %tempname%.log

echo - >%compname%.log
echo Compilando %compname%...
echo Compilación ----------------------------------------------------------- >>%compname%.log
date /T >>%compname%.log
time /T >>%compname%.log
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" "%compname%.iss" >>%compname%.log
echo End ----------------------------------------------------------- >>%compname%.log
echo - >>%compname%.log
echo Compilacion %compname% terminada

copy %compname%.log+%tempname%.log %compname%.log
del %tempname%.log

set tempname=
set compname=
pause