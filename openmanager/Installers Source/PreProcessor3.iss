;PREPROCESSOR 3
;Definicion de constantes para los programas

;**Constantes comunes en todos los programas**

;Nombre de la empresa
#define Company "JBC"
;Pagina web de la empresa
#define URL "http://www.jbctools.com/"
;Directorio donde estan los archivos del programa y los drivers y todo lo que tenga que ir en el instalador
#define SourcePath ".."
;Directorio donde saldra el archivo .exe instalador 
;If OutputDir is not a fully-qualified pathname, it will be treated as being relative to SourceDir
#define OutputPath "Installers"

;Nombre del instalador del VCP driver versión 6.5.3 o inferior
#define VCPdriver "CP210x_VCP_Win_XP_S2K3_Vista_7.exe"
;Version del driver VCP 
#define VCPdriverVer  "6.5.3"

;Nombre del instalador del VCP driver versión 6.6.1 x86 o superior
#define VCPdriverINNOX86 "CP210x_VCP_Win_XP_S2K3_Vista_7_8_81_x86.exe"
;Nombre del instalador del VCP driver versión 6.6.1 x64
#define VCPdriverINNOX64 "CP210x_VCP_Win_XP_S2K3_Vista_7_8_81_x64.exe"
;Version del driver VCP 
#define VCPdriverINNOVer  "670"

;Nombre del setup file de Freescale CDC para USB
#define FreescaleDriver "CDC_driver_x86_x64.inf"

;**Constantes diferentes para cada programa**

#ifdef MANAGER
  ;Nombre del programa
  #define Name "Manager"
  ;Version del programa, poner "U<2 digitos>.<4 digitos>" por ejemplo U01.6669, es como se guardara en el registro asi
  ;que hay que mantener el formato para poder comparar la version del registro con este valor.
  #define Version ManagerVer
  ;Icono para los accesos directos del escritorio
  #define desktopIconFileLarge "Manager128x128.ico"
  #define desktopIconFileSmall "Manager32x32.ico"
  ;Icono para los accesos directos del menu inicio
  #define startIconFile "Manager16x16.ico"

  ;Nombre del instalador del .NET Framework 4.0 Web Installer
  #define NETinstaller "dotNetFx40_Full_setup.exe"
#endif

#ifdef REMOTEMANAGER
  ;Nombre del programa
  #define Name "JBC Remote Manager"
  ;Version del programa, poner "U<2 digitos>.<4 digitos>" por ejemplo U01.6669, es como se guardara en el registro asi
  ;que hay que mantener el formato para poder comparar la version del registro con este valor.
  #define Version RemoteManagerVer
  ;Icono para los accesos directos del escritorio
  #define desktopIconFileLarge "Manager128x128.ico"
  #define desktopIconFileSmall "Manager32x32.ico"
  ;Icono para los accesos directos del menu inicio
  #define startIconFile "Manager32x32.ico"

  ;Nombre del instalador del .NET Framework 4.5.1 Web Installer
  #define NETinstaller "NDP451-KB2859818-Web.exe"
#endif

#ifdef STATIONCONTROLLERSERVICE
  ;Nombre del programa
  #define Name "JBC Station Controller Service"
  ;#define NameUpdater "JBC Host Updater Service"
  ;Version del programa, poner "U<2 digitos>.<4 digitos>" por ejemplo U01.6669, es como se guardara en el registro asi
  ;que hay que mantener el formato para poder comparar la version del registro con este valor.
  #define Version StationControllerServiceVer
  ;Icono para los accesos directos del escritorio
  #define desktopIconFileLarge "Manager128x128.ico"
  #define desktopIconFileSmall "Manager32x32.ico"
  ;Icono para los accesos directos del menu inicio
  #define startIconFile "Manager16x16.ico"

  ;Nombre del instalador del .NET Framework 4.0 Web Installer
  #define NETinstaller "dotNetFx40_Full_setup.exe"
#endif

#ifdef HOSTCONTROLLERSERVICE
  ;Nombre del programa
  #define Name "JBC Host Controller Service"
  ;Version del programa, poner "U<2 digitos>.<4 digitos>" por ejemplo U01.6669, es como se guardara en el registro asi
  ;que hay que mantener el formato para poder comparar la version del registro con este valor.
  #define Version HostControllerServiceVer
  ;Icono para los accesos directos del escritorio
  #define desktopIconFileLarge "Manager128x128.ico"
  #define desktopIconFileSmall "Manager32x32.ico"
  ;Icono para los accesos directos del menu inicio
  #define startIconFile "Manager16x16.ico"

  ;Nombre del instalador del .NET Framework 4.0 Web Installer
  #define NETinstaller "dotNetFx40_Full_setup.exe"
#endif

#ifdef OPENMANAGER
  ;Nombre del programa
  #define Name "JBC Open Manager"
  ;Version del programa, poner "U<2 digitos>.<4 digitos>" por ejemplo U01.6669, es como se guardara en el registro asi
  ;que hay que mantener el formato para poder comparar la version del registro con este valor.
  #define Version OpenManagerVer
#endif

#ifdef TRACECONTROL
  ;Nombre del programa
  #define Name "JBC Trace Control"
  ;Version del programa, poner "U<2 digitos>.<4 digitos>" por ejemplo U01.6669, es como se guardara en el registro asi
  ;que hay que mantener el formato para poder comparar la version del registro con este valor.
  #define Version TraceControlVer
  ;Icono para los accesos directos del escritorio
  #define desktopIconFileLarge "Manager128x128.ico"
  #define desktopIconFileSmall "Manager32x32.ico"
  ;Icono para los accesos directos del menu inicio
  #define startIconFile "Manager32x32.ico"

    ;Nombre del instalador del .NET Framework 4.0 Web Installer
  #define NETinstaller "dotNetFx40_Full_setup.exe"
#endif