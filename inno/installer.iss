#define MyAppName "GammaLauncher"
#define MyAppVersion "5.2.3"
#define MyAppPublisher "sebescudie"
#define MyAppURL "www.sebescudie.github.io"
#define MyAppExeName "gammalauncher_5.2.3_installer"


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
OutputDir=.
ArchitecturesAllowed=x64
WizardStyle=classic
PrivilegesRequired=admin
WizardSmallImageFile=img\ico.bmp
SetupIconFile=img\ico.ico

[Run]
Filename: {app}\{cm:AppName}.exe; Description: {cm:LaunchProgram,{cm:AppName}}; Flags: nowait postinstall skipifsilent
Filename: "https://github.com/sebescudie/GammaLauncher/wiki"; Description: "Open online documentation"; Flags: shellexec postinstall runmaximized skipifsilent

[CustomMessages]
AppName=GammaLauncher
LaunchProgram=Start GammaLauncher after finishing installation 


[Files]
Source: "output\GammaLauncher\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\GammaLauncher"; Filename: "{app}\GammaLauncher.exe"