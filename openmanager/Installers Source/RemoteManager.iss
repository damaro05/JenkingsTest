;Host Service installer
;defining the flag
#define REMOTEMANAGER

#define RemoteManagerVer "M03.2001"

; directorios donde est�n los programas/dlls compilados (seg�n versi�n que se quiera generar)
#define RemoteManagerDir "Manager 3.15.1\RemoteManager\Bin\Release"

;preprocessor constants
#include "PreProcessor3.iss"

;installer body, it is Inno Setup sections
#include ".\Body\RemoteManagerBody3.iss"
