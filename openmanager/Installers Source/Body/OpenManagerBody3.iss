[Setup]
;creates an installation log file in TEMP user folder
SetupLogging=yes

;install as Administrator
PrivilegesRequired=poweruser

;caracteristicas del soft
AppName={#Name}
AppVerName={#Name} {#Version}
AppPublisher={#Company}
AppPublisherURL={#URL}

;directorio donde se instalará por defecto
DefaultDirName="{tmp}"
;deshabilitar la pantalla Select Destination Location
DisableDirPage=yes
;deshabilitar el desinstalador
Uninstallable=no

;directorio donde encontrar los archivos del programa
SourceDir={#SourcePath}

;directorio del setup de salida
;If OutputDir is not a fully-qualified pathname, it will be treated as being relative to SourceDir
OutputDir="{#OutputPath}"

;nombre del archivo de salida
OutputBaseFilename={#Name} {#Version} Installer

;deshabilitar el reinicio del pc
RestartIfNeededByRun=no

;wizard images
WizardImageFile="Aditional Files\icons&img\wizardImageFile.bmp"
WizardSmallImageFile="Aditional Files\icons&img\wizardSmallImageFile.bmp"

[Types]
Name: "typical"; Description: "Typical installation"
Name: "custom";  Description: "Custom installation"

[Components]
Name: "typical"; Description: "Typical installation"; Types: typical
Name: "custom";  Description: "Custom installation";  Types: custom

[Tasks]
;preguntar si se pone icono en el escritorio
Name: remoteManager;     Description: "Install JBC Remote Manager";             Components: custom typical
Name: stationController; Description: "Install JBC Station Controller service"; Components: custom typical
Name: hostController;    Description: "Install JBC Host Controller service";    Components: custom;        Flags: unchecked

[Files]
Source: "Installers\{#RemoteManagerInstallerFile}.exe";     DestDir: "{app}"; Components: custom typical
Source: "Installers\{#StationControllerInstallerFile}.exe"; DestDir: "{app}"; Components: custom typical
Source: "Installers\{#HostControllerInstallerFile}.exe";    DestDir: "{app}"; Components: custom

[Run]
Filename: "{app}\{#StationControllerInstallerFile}"; Description: "Install JBC Station Controller service..."; Flags: runascurrentuser waituntilterminated; Tasks: stationController
Filename: "{app}\{#HostControllerInstallerFile}";    Description: "Install JBC Host Controller service...";    Flags: runascurrentuser waituntilterminated; Tasks: hostController
Filename: "{app}\{#RemoteManagerInstallerFile}";     Description: "Install JBC Remote Manager...";             Flags: runascurrentuser waituntilterminated; Tasks: remoteManager
