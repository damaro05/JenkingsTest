;Host Service installer
;defining the flag
#define TRACECONTROL

#define TraceControlVer "M03.2001"

; directorios donde est�n los programas/dlls compilados (seg�n versi�n que se quiera generar)
#define TraceControlDir "Library 3.15.1\JBCTraceControlSrv\Bin\Release"

;preprocessor constants
#include "PreProcessor3.iss"

;installer body, it is Inno Setup sections
#include ".\Body\TraceControlBody3.iss"
