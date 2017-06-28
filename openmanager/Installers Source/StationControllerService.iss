;Host Service installer
;defining the flag
#define STATIONCONTROLLERSERVICE

#define StationControllerServiceVer "M03.2001"

; directorios donde est�n los programas/dlls compilados (seg�n versi�n que se quiera generar)
#define StationControllerServiceDir "Library 3.15.1\JBCStationControllerSrv\Bin\Release"
#define UpdaterServiceDir "Library 3.15.1\JBCUpdateSrv\Bin\Release"

;preprocessor constants
#include "PreProcessor3.iss"

;installer body, it is Inno Setup sections
#include ".\Body\StationControllerServiceBody3.iss"
