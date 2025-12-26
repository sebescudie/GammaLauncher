$ErrorActionPreference = 'Stop'

$packageArgs = @{
packageName    = 'gammalauncher'
fileType       = 'exe'
url64bit       = 'https://github.com/sebescudie/GammaLauncher/releases/download/5.4.0/gammalauncher_5.4.0_win-x64_installer.exe'
checksum64     = 'dfe27ff053f8377b05919a3b284a3e406f2d4157eb43447f4d1200704114f472'
checksumType64 = 'sha256'
silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
validExitCodes = @(0)
}

Install-ChocolateyPackage @packageArgs