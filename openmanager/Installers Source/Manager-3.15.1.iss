;LabRegister installer
;defining the flag
#define MANAGER

#define ManagerVer "M03.1703"

; directorios donde están los programas/dlls compilados (según versión que se quiera generar)
#define ManagerDir "Manager 3.15.1\Manager\Bin\Release"
;#define ManRegisterDir "ManRegister 2.13.7\ManRegister\Bin\Release"
;#define LibraryDir "Library 2.14.7\JBC_Connect\Bin\Release"

;preprocessor constants
#include "PreProcessor3.iss"

;installer body, it is Inno Setup sections
#include ".\Body\ManagerBody2.iss"
