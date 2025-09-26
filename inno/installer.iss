#define MyAppName "GammaLauncher"
#define MyAppVersion "10.0.0"
#define MyAppPublisher "sebescudie"
#define MyAppURL "www.sebescudie.github.io"
#define MyAppExeName "gammalauncher_10.0.0_win-arm64_installer"

#include "CodeDependencies.iss"

[Setup]
AppId=GammmaLauncher
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
OutputBaseFilename={#MyAppExeName}
DefaultDirName={commonpf64}\GammaLauncher
DefaultGroupName=GammaLauncher
Uninstallable=yes
UninstallDisplayIcon={app}\GammaLauncher.exe
Compression=lzma2
OutputDir="..\artifacts\"
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=classic
PrivilegesRequired=admin
WizardSmallImageFile=..\img\ico.bmp
SetupIconFile=..\img\ico.ico

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_ForceX86 := False;
  Dependency_AddDotNet80Desktop;;
  Result := True;
end;

[Run]
Filename: {app}\{cm:AppName}.exe; Description: {cm:LaunchProgram,{cm:AppName}}; Flags: nowait postinstall skipifsilent
Filename: "https://github.com/sebescudie/GammaLauncher/wiki"; Description: "Open online documentation"; Flags: shellexec postinstall runmaximized skipifsilent

[CustomMessages]
AppName=GammaLauncher
LaunchProgram=Start GammaLauncher after finishing installation 


[Files]
Source: "..\artifacts\win-arm64\GammaLauncher\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\GammaLauncher"; Filename: "{app}\GammaLauncher.exe"