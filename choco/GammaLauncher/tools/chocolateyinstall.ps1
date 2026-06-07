$ErrorActionPreference = 'Stop'

$packageArgs = @{
packageName    = 'gammalauncher'
fileType       = 'exe'
url64bit       = 'https://github.com/sebescudie/GammaLauncher/releases/download/5.4.2/gammalauncher_5.4.2_win-x64_installer.exe'
checksum64     = '3e561a10b1cd8aac7c5aeb4fc75f1011a379fbf2541f04fc77b6fe6c35fd36cc'
checksumType64 = 'sha256'
silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
validExitCodes = @(0)
}

Install-ChocolateyPackage @packageArgs