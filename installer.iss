#define MyAppName "GammaLauncher"
#define MyAppVersion "5.0.0"
#define MyAppPublisher "sebescudie"
#define MyAppURL "www.sebescudie.github.io"
#define MyAppExeName "gammalauncher_5.0.0"


[Setup]
AppId={{CB22A910-7C8F-4884-BC15-BC22602713A9}
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
WizardSmallImageFile=ico.bmp
SetupIconFile=ico.ico

[Run]
Filename: {app}\{cm:AppName}.exe; Description: {cm:LaunchProgram,{cm:AppName}}; Flags: nowait postinstall skipifsilent
Filename: "https://github.com/sebescudie/GammaLauncher/wiki"; Description: "Open online documentation"; Flags: shellexec postinstall runmaximized skipifsilent

[CustomMessages]
AppName=GammaLauncher
LaunchProgram=Start GammaLauncher after finishing installation 


[Files]
Source: "build\GammaLauncher\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\GammaLauncher"; Filename: "{app}\GammaLauncher.exe"