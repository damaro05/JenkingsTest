;Open Manager installer
;defining the flag
#define OPENMANAGER

#define OpenManagerVer "M03.2001"

;define installers
#define RemoteManagerInstallerFile "JBC Remote Manager M03.2001 Installer"
#define StationControllerInstallerFile "JBC Station Controller Service M03.2001 Installer"
#define HostControllerInstallerFile "JBC Host Controller Service M03.2001 Installer"

;preprocessor constants
#include "PreProcessor3.iss"

;installer body, it is Inno Setup sections
#include ".\Body\OpenManagerBody3.iss"
