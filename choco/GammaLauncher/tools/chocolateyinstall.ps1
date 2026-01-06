$ErrorActionPreference = 'Stop'

$packageArgs = @{
packageName    = 'gammalauncher'
fileType       = 'exe'
url64bit       = 'https://github.com/sebescudie/GammaLauncher/releases/download/5.4.1/gammalauncher_5.4.1_win-x64_installer.exe'
checksum64     = 'd0958110ed948381c8a221de223386f620ae3d0a22c72c9aea6654a205a6d24d'
checksumType64 = 'sha256'
silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
validExitCodes = @(0)
}

Install-ChocolateyPackage @packageArgs