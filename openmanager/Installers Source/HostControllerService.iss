;Host Controller Service installer
;defining the flag
#define HOSTCONTROLLERSERVICE

#define HostControllerServiceVer "M03.2001"

; directorios donde están los programas/dlls compilados (según versión que se quiera generar)
#define HostControllerServiceDir "Library 3.15.1\JBCHostControllerSrv\Bin\Release"
#define UpdaterServiceDir "Library 3.15.1\JBCUpdateSrv\Bin\Release"

;preprocessor constants
#include "PreProcessor3.iss"

;installer body, it is Inno Setup sections
#include ".\Body\HostControllerServiceBody3.iss"
