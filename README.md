# GammaLauncher

<p align="center">
<img src="img/capture.png" title="" alt="Gammalauncher Screenshot" width="259">
</p>

_A simple launcher for vvvv gamma_

[![vvvv](https://img.shields.io/static/v1?label=MADE%20WITH&message=VVVV&color=191919&style=for-the-badge)](https://vvvv.org/) ![Chocolatey Version](https://img.shields.io/chocolatey/v/gammalauncher?include_prereleases&style=for-the-badge&logo=chocolatey&link=https%3A%2F%2Fcommunity.chocolatey.org%2Fpackages%2Fgammalauncher)


## Was ist das

GammaLauncher is a simple application that allows to launch vvvv gamma instances with specific arguments from a simple GUI.

## Installation
The latest release is available [here](https://github.com/sebescudie/GammaLauncher/releases/latest). You can choose between an installer and a portable version.

## Usage

### Settings

Open the app and go to the _Settings_ tab. You'll find the following sections

#### Launcher Settings

- vvvv installations folder: this is where the launcher will look for your installed vvvversions. It defaults to `C:\Program Files\vvvv`
- Nuget override : if you want to override the default nuget folder with your own, specify it here
- Extra args : arbitrary arguments the launcher will use when starting a vvvv instance

#### Package repositories

All folders you add in this section will be used as package sources. More information [here](https://thegraybook.vvvv.org/reference/extending/contributing.html#source-package-repositories)

#### Editable packages

Allows listed packages to be modified. Otherwise, vvvv precompiles them and they're read-only. More information [here](https://thegraybook.vvvv.org/reference/language/compilation.html#read-only-packages).

### Updates

When the app starts, it looks online for the most recent vvvv preview and stable builds, both for `win-x64` and `arm64`. If a more recent version is available online, the _Updates_ tab will show a `*` character. You can then browse the available builds and click the _Install_ button to download and start the installer. A _Changes_ button allows to view the changelog for this version right in the app. You can also click _Check for updates_ or press <kbd>SHIFT + F5</kbd> at any time to maually look for updates.

### Opening documents

You can drag and drop any VL document from your file browser on the launcher to open it in a new vvvv instance.

## Features

- Run vvvv instances with any combination of [command line arguments](https://thegraybook.vvvv.org/reference/hde/commandline-arguments.html#commandline-arguments) from a simple GUI
- Install and uninstall vvvversions with a single click
- Quickly access useful folders such as installed nugets, default nugets, package sources and so on
- Quickly kill all running vvvv instances
- Quickly start new instances with a specific document by dropping it on the launcher

## Contributing

All suggestions and pull requests are welcome, don't hesitate to report problems and share suggestions in the issues or on the [vvvv forum](http://www.discourse.vvvv.org).

## Building

- Write the version you're aiming to build in `Version.props`
- `cd` in the `📂 /nuke` folder. You'll be able to do the following actions 

### Clean

This target will clean all artifacts from previous builds. To run it, do

```
build.cmd clean
```

### Compile

This target compiles GammaLauncher in both `win-x64` and `arm64` directly from the command line. The version number will be the one you specified in `Version.props`.

```
nuke compile --compilerpath "C:\path\to\vvvvc.exe"
```

Executing this target will also run the `clean` target described above under the hood.

### Build installers

This target packages the binaries from the previous step in cool installers. For this one, you'll have to have [Inno Setup](https://jrsoftware.org/isdl.php) installed on your machine!
This target will obviously run `clean` and `compile` under the hood for you. To run it, do the following. Agzin, the version number will be the one you specified in `Version.props`

```
nuke buildinstallers --compilerpath "C:\path\to\vvvvc.exe"
```