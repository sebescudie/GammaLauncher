$ErrorActionPreference = 'Stop'

$packageArgs = @{
packageName    = 'gammalauncher'
fileType       = 'exe'
url64bit       = 'https://github.com/sebescudie/GammaLauncher/releases/download/5.3.2/gammalauncher_5.3.2_win-x64_installer.exe'
checksum64     = '39e3bd87a8f87415b0e7218cba32c6a90cabdcb431105009d4fcdab2dcecf504'
checksumType64 = 'sha256'
silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
validExitCodes = @(0)
}

Install-ChocolateyPackage @packageArgs