; -- 64Bit.iss --
; Demonstrates installation of a program built for the x64 (a.k.a. AMD64)
; architecture.
; To successfully run this installation and the program it installs,
; you must have a "x64" edition of Windows.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
AppName=GammaLauncher
AppVersion=5.0.0
WizardStyle=modern
DefaultDirName={commonpf64}\GammaLauncher
DefaultGroupName=GammaLauncher
UninstallDisplayIcon={app}\GammaLauncher.exe
Compression=lzma2
OutputDir=.
ArchitecturesAllowed=x64
OutputBaseFilename=gammalauncher_5.0.0_setup

[Files]
Source: "{userdocs}vvvv\gamma-preview\Exports\GammaLauncher\*"; DestDir: "{commonpf64}\GammaLauncher"; Flags: external

[Icons]
Name: "{group}\GammaLauncher"; Filename: "{app}\GammaLauncher.exe"