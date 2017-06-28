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

;directorio por defecto en el menu inicio para el programa, es la variable {group} usada para definir los path de los iconos del menu inicio
DefaultGroupName={#Company}

;directorio donde se instalará por defecto
DefaultDirName={code:setDefaultDir}

;directorio donde encontrar los archivos del programa
SourceDir={#SourcePath}

;indicamos versiones de windows soportadas, el 0 es que no es soportado windows que no sea NT ( del XP para arriba todos son NT)
;y el 5.1 es winXP.
minVersion=0, 5.1

;archivo de licencia, lo busca en Source. Tiene que estar en el directorio definido en la constante "SourcePath"
LicenseFile=License_Manager.txt

;directorio del setup de salida
OutputDir="{#OutputPath}"

;nombre del archivo de salida
OutputBaseFilename={#Company} {#Name} {#Version} Installer

;instalación en 64 bits
ArchitecturesInstallIn64bitMode = x64 ia64

[Tasks]
;preguntar si se pone icono en el escritorio
Name: desktopIcon; Description: "Create a &desktop icon"; Flags: unchecked

[Dirs]
;añadir subdirectorio "All users\Application Data\JBC\ManRegister\Templates", este subdirectorio es donde iran los templates del ManRegister
;{commonappdata} es "C:\Documents and Settings\All users\Datos de programa" que no aparece para un usuario limitado              
;{localappdata} es "C:\Documents and Settings\Eduardo\Configuración local\Datos de programa" que es solo del usuario
;{commondocs} es "C:\Documents and Settings\All Users\Documentos"
;{cf} es "C:\Archivos de programa\Archivos comunes"
Name: "{cf}\JBC\Manager"; Permissions: everyone-modify; AfterInstall: MyMoveXmlStationList 
Name: "{cf}\JBC\Manager\Templates"; Permissions: everyone-modify; AfterInstall: MyMoveTemplates

[Files]
;este es el driver de VCP para winXP a win7, se incluye para el proceso de instalación pero se borra automaticamente.
Source: "Aditional Files\{#VCPdriver}"; DestDir: "{app}"; Flags: dontcopy 
;este es el driver de VCP para winXP a win8.1, se incluye para el proceso de instalación y se copia por si el usuario quiere reinstalarlo.
Source: "Aditional Files\{#VCPdriverINNOX86}"; DestDir: "{app}\drivers\Silicon";  
;este es el driver de VCP para winXP a win8.1, se incluye para el proceso de instalación y se copia por si el usuario quiere reinstalarlo.
Source: "Aditional Files\{#VCPdriverINNOX64}"; DestDir: "{app}\drivers\Silicon"; 
;este es el instalador del .NET Framework 3.5, se incluye para el proceso de instalación pero se borra automaticamente.
Source: "Aditional Files\{#NETinstaller}"; DestDir: "{app}"; Flags: dontcopy
;este es el driver de Freescale CDC para x86 y x64, se deja instalado porque el usuario tiene que encontrarlo cuando se lo pida el sistema.
Source: "Aditional Files\{#FreescaleDriver}"; DestDir: "{app}\drivers\Freescale";

;incluir archivos a instalar, path relativas a SourceDir
Source: "{#ManagerDir}\Manager.exe"; DestDir: "{app}"; Check: not Is64BitInstallMode
Source: "{#ManagerDir}\Manager.exe"; DestDir: "{app}"; Check: Is64BitInstallMode
Source: "{#ManagerDir}\Manager.exe.config"; DestDir: "{app}"
Source: "{#ManagerDir}\es\Manager.resources.dll"; DestDir: "{app}\es"; Check: not Is64BitInstallMode
Source: "{#ManagerDir}\es\Manager.resources.dll"; DestDir: "{app}\es"; Check: Is64BitInstallMode

Source: "{#ManagerDir}\JBC_Connect.dll"; DestDir: "{app}"; Check: not Is64BitInstallMode
Source: "{#ManagerDir}\JBC_Connect.dll"; DestDir: "{app}"; Check: Is64BitInstallMode

Source: "{#ManagerDir}\ManRegister.dll"; DestDir: "{app}"; Check: not Is64BitInstallMode
Source: "{#ManagerDir}\ManRegister.dll"; DestDir: "{app}"; Check: Is64BitInstallMode
Source: "{#ManagerDir}\es\ManRegister.resources.dll"; DestDir: "{app}\es"; Check: not Is64BitInstallMode
Source: "{#ManagerDir}\es\ManRegister.resources.dll"; DestDir: "{app}\es"; Check: Is64BitInstallMode

Source: "Aditional Files\JBC Manager User's Manual.pdf"; DestDir: "{app}"

Source: "Aditional Files\icons&img\{#desktopIconFileLarge}"; DestDir: "{app}"
Source: "Aditional Files\icons&img\{#desktopIconFileSmall}"; DestDir: "{app}"
Source: "Aditional Files\icons&img\{#startIconFile}"; DestDir: "{app}"

[Icons]
;iconos de acceso directo en el menu inicio
Name: "{group}\{#Name}\{#Name}"; Filename: "{app}\Manager.exe"; WorkingDir: "{app}"; Comment: "Executes the JBC Manager"; IconFileName: "{app}\{#startIconFile}"
Name: "{group}\{#Name}\User's Manual"; Filename: "{app}\JBC Manager User's Manual.pdf"; WorkingDir: "{app}"; Comment: "The Manager User's Manual";

;acceso directo del desinstalador
Name: "{group}\{#Name}\Uninstall"; Filename: "{uninstallexe}"; WorkingDir: "{app}"; Comment: "Uninstalls the JBC Manager"; Flags: createonlyiffileexists

;iconos de acceso directo en el escritorio
;este es para instalaciones nuevas y depende de si el usuario ha seleccionado poner el icono
Name: "{userdesktop}\{#Name}"; Filename: "{app}\Manager.exe"; WorkingDir: "{app}"; Comment: "Executes the JBC Manager"; Tasks: desktopIcon; IconFileName: {code:setDesktopIconFileName}

;este es para instalaciones de reparación, actualización o downgrades. Depende de si el usuario habia seleccionado en la primera instalación el icono
Name: "{userdesktop}\{#Name}"; Filename: "{app}\Manager.exe"; WorkingDir: "{app}"; Comment: "Executes the JBC Manager"; Check: createDesktopIcon; IconFileName: {code:setDesktopIconFileName}

[Registry]
;añadiendo una entrada en el registro para almacenar el directorio de instalación
;hacemos una llave generica para los programas JBC, al desinstalar se borrará solo si esta vacia
Root: HKLM; Subkey: "Software\{#Company}"; Flags: uninsdeletekeyifempty

;añadimos el programa en cuestion, se borrara al desinstalar
Root: HKLM; Subkey: "Software\{#Company}\{#Name}"; Flags: uninsdeletekey

;añadimos el valor del directorio de instalacion
Root: HKLM; Subkey: "Software\{#Company}\{#Name}\Settings"; ValueType: string; ValueName: "Path"; ValueData: "{app}"

;añadimos el valor de la version del programa
Root: HKLM; Subkey: "Software\{#Company}\{#Name}\Settings"; ValueType: string; ValueName: "Version"; ValueData: {#Version}

;añadimos una entrada indicando si se ha seleccionado poner icono en el escritorio
;esta primera es para la primera instalación y depende de si el usuario ha seleccionado o no poner el icono
Root: HKLM; Subkey: "Software\{#Company}\{#Name}\Settings"; ValueType: string; ValueName: "desktopIcon"; ValueData: ""; Tasks: desktopIcon;

;este es para instalaciones de reparación, actualización o downgrades. Depende de si el usuario habia seleccionado en la primera instalación el icono
Root: HKLM; Subkey: "Software\{#Company}\{#Name}\Settings"; ValueType: string; ValueName: "desktopIcon"; ValueData: ""; Check: createDesktopIcon

;este es por si el PC no tiene puertos serie, hay que crear la entrada del registro de los puertos por que hasta que no se
;conecte la primera estación no se generara la entrada y el programa fallaria.
Root: HKLM; Subkey: "HARDWARE\DEVICEMAP\SERIALCOMM"; Flags: uninsdeletekeyifempty

[UninstallDelete]
;si no se ha hecho una instalacion nueva se tiene que indicar que borre los accesos directos porque
;al sobreescribir la instalacion se los ha encontrado hechos y por lo tanto decide no borrarlos.
Type: dirifempty; Name: "{group}"
Type: filesandordirs; Name: "{group}\{#Name}"
Type: files; Name: "{userdesktop}\{#Name}"

[Code]
const
  { const used by alreadyInstalled() to indicate if there's a current installation }
  NOT_INSTALLED = 0;
  INSTALLED = 1;

  { const used by installationMode var to know which type of installation must be performed }
  NEW_INSTANCE = 0;
  REPAIR_INSTANCE = 1;
  UPDATE_INSTANCE = 2;
  OLD_INSTANCE = 3;
  
  { Message send to the form to terminate it, used in quit() procedure }
  WM_QUIT = $0012;

  { Silicon Labs VCP drivers registry entries version 6.5 or lower, used in installVCPdriver() - InstallShield}
  VCPregKey32bit = 'SOFTWARE\Silicon Laboratories, Inc.\Silicon Laboratories Driver Installations\VCP CP210x Cardinal\SLABCOMM&10C4&EA60';
  VCPregKey64bit = 'SOFTWARE\Wow6432Node\Silicon Laboratories, Inc.\Silicon Laboratories Driver Installations\VCP CP210x Cardinal\SLABCOMM&10C4&EA60';
  { Silicon Labs VCP drivers registry entries version 6.6 or greater, used in installVCPdriver() - creted INNO installation}
  VCPregKey32bitINNO = 'SOFTWARE\Silicon Laboratories, Inc.\Silicon Laboratories Driver Installations\VCP CP210x INNO';
  VCPregKey64bitINNO = 'SOFTWARE\Wow6432Node\Silicon Laboratories, Inc.\Silicon Laboratories Driver Installations\VCP CP210x INNO';

  VCPregValue = 'DriverVersion';
  VCPversion = '{#VCPdriverVer}';
  VCPversionINNO = '{#VCPdriverINNOVer}';
  
  { .NET Framework 3.5 registry entries, used in installNETfw() }
  NETregKey = 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5';
  NETregValue = 'Install';

var
  { This global var is used to decide which type of installation must be done. Its values are
  defined as constants in the const section. The installation modes are:
  new ->          there's no current installation
  repair ->       there's a current installation with the same version then the installer
  update ->       there's a current installation with an older version from the installer
  old ->          there's a current installation with a newer version from the installer }
  installationMode: Integer;
  
  { This var indicates if the desktop icon is required }
  desktopIcon: Boolean;

  { This var stores the current installation directory if there's a current installation }
  curAppPath: String;

  { This var stores the current installation version if there's a current installation }
  curAppVersion: String;
  
  { This is the installation mode information page }
  modePage: TOutputMsgWizardPage;
  
  { This page is showed during the VCP and .NET installations to inform user }
  waitPage: TOutputProgressWizardPage;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function checks if the installation mode is a new instance or not. It's used in [Icons] section
to avoid creating shortcuts if the program is already installed because the shortcuts may be created.
RETURN:
  A boolean indicating if is new instance (True) os not (False) }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function isNewInstance(): Boolean;
begin
  result := ( installationMode = NEW_INSTANCE);
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function is used in the [Setup] section to set the value of DefaultDirName. The directory where
to install the application is selected by the user or passed as a parametter when a new instance is being
installed, but if is not a new instance the directory is the same as the current installed instance.
This function manages this feature by assigning the current instance path or a recommended path for new instance.
RETURN:
  A string with the installation path. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function setDefaultDir( param: String): String;
begin
  if isNewInstance then begin
    Result := ExpandConstant('{pf}') + '\{#Company}\{#Name}';
  end
  else begin
    Result := curAppPath;
  end;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function checks the OS and depending on the version indicates the desktop icon to use.
RETURN:
  A string indicating which desktop icon to use }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function setDesktopIconFileName( param: String): String;
begin
  if InstallOnThisVersion('0,5','0,6') = irInstall then begin
    { if winXP small icon }
    Result := ExpandConstant('{app}') + '\{#desktopIconFileSmall}';
  end
  else begin
    { if win Vista/7 large icon }
    Result := ExpandConstant('{app}') + '\{#desktopIconFileLarge}';
  end
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function simply returns the value of the desktopIcon global var. If returns true the desktop
icon must be created and its registry entry, otherwise retunrs false.
RETURN:
  A boolean indicating if the desktop icon exists in the previous installation. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function createDesktopIcon(): Boolean;
begin
  Result := desktopIcon;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function checks in the registry for the path and the version of the program, if found return
the const INSTALLED and set the global "curAppPath" and "curAppVersion" to installed program path and version. If
not found return the const NOT_INSTALLED.
RETURN:
  An integer indicating if there's a current program ( INSTALLED ) or not ( NOT_INSTALLED) }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function alreadyInstalled(): Integer;
begin
  { getting the current installed program path, if not exist the registry entry there's no current program installed }
  if RegQueryStringValue( HKLM, 'Software\{#Company}\{#Name}\Settings', 'Path', curAppPath) and
     RegQueryStringValue( HKLM, 'Software\{#Company}\{#Name}\Settings', 'Version', curAppVersion) then begin
    { Program is already installed, initializing the desktopIcon var and indicating already installed }
    desktopIcon := RegValueExists( HKLM, 'Software\{#Company}\{#Name}\Settings', 'desktopIcon');
    Result := INSTALLED;
  end
  else begin
    { There's no path registry entry, there's no installed program }
    Result := NOT_INSTALLED;
  end;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function just creates a wizard info page indicating that a current instance of the program is
installed and the procedure to follow depending on if is a repair, update or old installing mode. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
procedure createInstallModePage();
var
  title: String;
  description: String;
  text: String;

begin
  description := '{#Name} is already installed in your computer';
  text := 'A previous installation of {#Name} has been detected in next folder: ' #13#13 +
          'Path: ' + curAppPath + #13#13 +
          'The current installed {#Name} version is: '#13#13 +
          'Version: ' + curAppVersion + #13#13;

  case installationMode of
    UPDATE_INSTANCE:
      begin
        title := 'Update Program Version';
        text := text + 'By clicking on "Next" button {#Name} will be updated to version: ' #13#13 + ' Version: {#Version}';
      end;
    REPAIR_INSTANCE:
      begin
        title := 'Repair Program Version';
        text := text + 'By clicking on "Next" button {#Name} current installation will be repaired';
      end;
    OLD_INSTANCE:
      begin
        title := 'Old Installer Version';
        text := text + 'This installer version is: ' #13#13 + 'Version: {#Version}' + #13#13 'The installer has an older version, ' +
                       'it is recommended to not install this older version, however by clicking "Next" button the old {#Name} version will be installed (not recommended)';
      end;
  end;
  
  modePage := CreateOutputMsgPage(wpLicense, title, description, text);
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function launchs the current installation uninstaller in silent mode. Is used to uninstall the
program before reinstalling it by repair, update or old installation modes. It is the uninstall part of
a reinstall procedure. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
procedure uninstallCurrentInstance();
var
  uninsResult: Integer;
  uninsData: TFindRec;
  
begin
  { launching the current unins*.exe in silent mode }
  if FindFirst( curAppPath + '\unins*.exe', uninsData) then begin
    if not Exec(curAppPath + '\' + uninsData.Name, '/VERYSILENT', '', SW_HIDE, ewWaitUntilTerminated, uninsResult) then begin
      msgbox('An unexpected error has ocurred' + SysErrorMessage(uninsResult) + #13 +
             'If it''s not the first time this error appear contact with JBC suport at {#URL}', mbCriticalError, MB_OK);
    end;
  end;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function just aborts the setup, when called the setup is aborted immediately }
////////////////////////////////////////////////////////////////////////////////////////////////////////
procedure quit();
begin
  PostMessage(WizardForm.Handle, WM_QUIT, 0, 0);
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function checks if the VCP drivers are installed and if not install it. If an error ocur or
the driver installation fails this function indicates it and aborts the setup. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
procedure installVCPdriver();
var
  versionNotFount: string;
  resultCode: Integer;
  version32: String;
  version64: String;
  version32INNO: String;
  version64INNO: String;
  VCPdriverFileName: String;
  VCPdriverFileNameINNO: String;
  sMsg: String;
  bInstallDriver: boolean;

begin
  versionNotFount := '-1.0';
  if Is64BitInstallMode then
    begin
    VCPdriverFileName := '{#VCPdriver}';
    VCPdriverFileNameINNO := '{#VCPdriverINNOX64}';
    end
  else
    begin
    VCPdriverFileName := '{#VCPdriver}';
    VCPdriverFileNameINNO := '{#VCPdriverINNOX86}';
    end;

  { setting an imposible version value }
  version32 := versionNotFount;
  version64 := versionNotFount;
  version32INNO := versionNotFount;
  version64INNO := versionNotFount;
  bInstallDriver := false; 

  { checking if the VCP are already installed, getting the versoin of 32bit and 64bit. If not installed the versionNN string
    won't be changed. It is obvious that in a 32 bit system the 64bit version will not be and viceversa. }
  RegQueryStringValue( HKLM, VCPregKey32bit, VCPregValue, version32);
  RegQueryStringValue( HKLM, VCPregKey64bit, VCPregValue, version64);
  RegQueryStringValue( HKLM, VCPregKey32bitINNO, VCPregValue, version32INNO);
  RegQueryStringValue( HKLM, VCPregKey64bitINNO, VCPregValue, version64INNO);

  if not (CompareText( version32INNO, VCPversionINNO) = 0)  and not ( CompareText( version64INNO, VCPversionINNO) = 0) then begin
    // not instaled last version of driver
    // "update" or "install" message
    if (CompareText( version32, versionNotFount) = 0) and ( CompareText( version64, versionNotFount) = 0) and 
       (CompareText( version32INNO, versionNotFount) = 0) and ( CompareText( version64INNO, versionNotFount) = 0) then begin
      bInstallDriver := true;
      sMsg := 'The Virtual COM Port(VCP) drivers are required in order to use this program.' #13 +
              'The VCP setup will be launched, follow the steps to complete the installation';
    end else begin
      if ((CompareText(version32INNO, versionNotFount) <> 0) and (CompareText(version32INNO, VCPversionINNO) > 0)) or
         ((CompareText(version64INNO, versionNotFount) <> 0) and (CompareText(version64INNO, VCPversionINNO) > 0)) then begin
         // do not install, there is a newer version installed
         bInstallDriver := false;
      end else begin
          bInstallDriver := true;
          sMsg := 'The Virtual COM Port(VCP) drivers will be updated to last version.' #13 +
                  'The VCP setup will be launched, follow the steps to complete the installation';
      end;
    end;
  end;

  if (bInstallDriver = true) then begin
    { last driver are not installed, installing them }
    msgbox(sMsg, mbInformation, MB_OK);

    { Indicating what is being installed in the wait page }
    waitPage.SetText('Installing the VCP drivers...','');
    waitPage.Show();

    { Extracting the driver installer as a temporary file }
    ExtractTemporaryFile(VCPdriverFileNameINNO);
    
    { Executing the VCP driver }
    if Exec( ExpandConstant('{tmp}\' + VCPdriverFileNameINNO), '','', SW_SHOW, ewWaitUntilTerminated, resultCode) then begin
      { Checking execution result, if 0 driver installer not completed }
      if not ( resultCode = 0) then begin
        msgbox('The VCP drivers couldn''t have been installed. Aborting installation.' , mbError, MB_OK);
        waitPage.Hide();
        quit();
      end else begin
        waitPage.Hide();
      end;
    end else begin
      { Cannot execute the driver installer }
      msgbox('An unexpected error has ocurred: ' + SysErrorMessage(resultCode) + #13 +
             'If it''s not the first time this error appear contact with JBC suport at {#URL}', mbCriticalError, MB_OK);
      waitPage.Hide();
      quit();
    end;
  end;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ This function checks if the .NET framework 3.5 is installed and if not install it. If an error ocur or
the application installation fails this function indicates it and aborts the setup. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
procedure installNETfw();
var
  resultCode: Integer;
  NETinstalled: Cardinal;
  
begin
  NETinstalled := -1;
  { checking if the .NET is already installed }
  if not RegQueryDWordValue( HKLM, NETregKey, NETregValue, NETinstalled) or not ( NETinstalled = 1) then begin
    { The .NET is not installed, installing it }
    msgbox('The .NET Framework 3.5 is required in order to use this program.' #13 +
           'The .NET Framework 3.5 setup will be launched, follow the steps to complete the installation', mbInformation, MB_OK);

    { Indicating what is being installed in the wait page }
    waitPage.SetText('Installing the .NET Framework 3.5...','');
    waitPage.Show();

    { Extracting the driver installer as a temporary file }
    ExtractTemporaryFile( '{#NETinstaller}');

    { Executing the .NET Framework 3.5 installer }
    if Exec( ExpandConstant('{tmp}\{#NETinstaller}'), '','', SW_SHOW, ewWaitUntilTerminated, resultCode) then begin
      { Checking execution result, if 0 installation not completed }
      if not ( resultCode = 0) then begin
        msgbox('The .NET Framework 3.5 couldn''t have been installed. Aborting installation.' , mbError, MB_OK);
        waitPage.Hide();
        quit();
      end else begin
        waitPage.Hide();
      end;
    end else begin
      { Cannot execute the .NET Framework 3.5 installer }
      msgbox('An unexpected error has ocurred: ' + SysErrorMessage(resultCode) + #13 +
             'If it''s not the first time this error appear contact with JBC suport at {#URL}', mbCriticalError, MB_OK);
      waitPage.Hide();
      quit();
    end;
  end;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ Inno Setup internall function called when initializing the setup. Used to initially decide which
installation type is required }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function InitializeSetup(): Boolean;
begin
  { checking if the program is already installed }
  if alreadyInstalled() = INSTALLED then begin
    { The program is already installed, checking the version to update, repair or old }
    if CompareText( curAppVersion, '{#Version}') < 0 then begin installationMode := UPDATE_INSTANCE; end;  // installer has newer version
    if CompareText( curAppVersion, '{#Version}') = 0 then begin installationMode := REPAIR_INSTANCE; end;  // installer has the same version
    if CompareText( curAppVersion, '{#Version}') > 0 then begin installationMode := OLD_INSTANCE; end;     // installer has an old version
  end
  else begin
    installationMode := NEW_INSTANCE;  // there's no current installation
  end;
  Result := True;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ Inno Setup internall function called when initialized the wizard pages. Used to create the wait page
and install mode page. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
procedure InitializeWizard();
begin
  { if repair, update or old required adding a page after License to indicate this procedure }
  if not isNewInstance() then begin createInstallModePage(); end;
  
  { creating the wait page when installing VCP drivers or .NET framework 3.5 }
  waitPage := CreateOutputProgressPage( 'Installing required software...', 'Please wait while required software setup finishes.' #13 +
                                        'This operation may take some minutes.');
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ Inno Setup internall function called each time a new page of the wizard is going to appear, if this
function returns true the page would not appear or, what is the same, is skiped. Used to skip some
wizard pages not used depending on the installation mode. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function ShouldSkipPage(PageID: Integer): Boolean;
begin
  if ( (PageID = wpSelectDir) or (PageID = wpSelectProgramGroup) or (PageID = wpReady) or (PageID = wpSelectTasks))
     and ( not isNewInstance()) then begin
    Result := True;
  end;
end;

////////////////////////////////////////////////////////////////////////////////////////////////////////
{ Inno Setup internall function called when the next button of the current wizard page is pressed.
Used to start the repair, update or old previous uninstall before reinstall and to launch the
VCP and .NET setups. }
////////////////////////////////////////////////////////////////////////////////////////////////////////
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  if not isNewInstance() then begin
    if CurPageID = modePage.ID then begin
      { uninstalling the current program installation }
      uninstallCurrentInstance();
    end;
  end;
  if CurPageID = wpWelcome then begin
    { lanching the VCP drivers }
    installVCPdriver();
    
    { Then launching the .NET Framework 3.5 }
    installNETfw();
  end;
  Result := True;
end;

procedure MyMoveXmlStationList();
begin
     // move from old location
     if FileExists(curAppPath + '\StationGroups.xml') then
        begin
        if FileCopy(curAppPath + '\StationGroups.xml', ExpandConstant('{cf}') + '\JBC\Manager\StationGroups.xml', true) then
           begin 
           DeleteFile(curAppPath + '\StationGroups.xml');
           end;
        end;

end;

procedure MyMoveTemplates();
var
  sPath_XP: string;
  sPath_W7: string;
  sPath: string;
  sFile: string;
  sTargetPath: string;
  FilesFound: Integer;
  FindRec: TFindRec;
begin
     // move from old location
     sPath_XP := ExpandConstant('{commonappdata}') + '\..\JBC\LabRegister\Templates';
     sPath_W7 := ExpandConstant('{commonappdata}') + '\JBC\LabRegister\Templates';
     sTargetPath := ExpandConstant('{cf}') + '\JBC\Manager\Templates'
     if DirExists(sPath_XP) then
        sPath := sPath_XP;
     if DirExists(sPath_W7) then
        sPath := sPath_W7;
 
     if DirExists(sPath) then
        begin
        // "{cf}\JBC\Manager\Templates"
        FilesFound := 0;
        if FindFirst(sPath + '\*', FindRec) then begin
          try
            repeat
              sFile := FindRec.Name;
              if FileCopy(sPath + '\' + sFile, sTargetPath + '\' + sFile, true) then
                 begin 
                 DeleteFile(sPath + '\' + sFile);
                 end;
            until not FindNext(FindRec);
          finally
            FindClose(FindRec);
          end;
        end;
    end;

end;
