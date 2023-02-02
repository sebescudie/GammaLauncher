# GammaLauncher

<p align="center">
<img src="capture.png" title="" alt="GitHub Logo" width="259">
</p>

_A simple launcher for vvvv gamma_

[![vvvv](https://img.shields.io/static/v1?label=MADE%20WITH&message=VVVV&color=191919&style=for-the-badge)](https://visualprogramming.net/)

## Was ist das

GammaLauncher is a simple application that allows to launch vvvv gamma instances with specific arguments from a simple GUI.

## Usage

When the application first starts, it generates a `settings.xml` file in `%APPDATA\Local\GammaLauncher`. This file contains path to your gamma installations (should be `C:\Program Files\vvvv`), as well as your repositories folder, if you're using that feature. You can customize those paths if needed by editing the settings file. To quickly locate the settings file, press <kbd>CTRL</kbd> + <kbd>,</kbd>.

Then, simply pick the specific gamma version you wanna start from the dropdown menu, and tick the options you wanna use :

- Allow multiple : allows to start more than one `vvvv.exe` at once
- Use repositories : this will ovewrite packages in the default package folder with the ones in your repositories folder, if you specify one. This is usefull if you're building libraries for gamma.
- No cache : doesn't load .vl.dll files and compiles everything from scratch

## Features

- Two buttons at the bottom of the app allow you to quickly access your `package-repositories` folder and installed nugets
- A third one allows to quickly kill all running vvvv instances
- Pressing <kbd>CTRL</kbd>+<kbd>R</kbd> refreshes your installed versions list
- Pressing <kbd>CTRL</kbd>+<kbd>U</kbd> starts the uninstaller associated with the version currently selected in the Dropdown
- Pressing <kbd>CTRL</kbd>+<kbd>,</kbd> takes you to the application's folder so you can quickly edit your settings file
- Mouse scrolling over the vvvversion dropdown allows you cycle through your installed versions without clicking/unfolding it
- If you have _Allow Multiple_ set to true, drag and dropping a `.vl` file on the launcher will open it in a new vvvv instance with all set parameters.
- A burger menu allows to check for two things :
  - If a new version of vvvv gamma exists online. If so, you'll be able to download and install it directly from the launcher with a single click
  - If a new version of GammaLauncher exists online. If so, clicking on the version number takes you to the download page on Github.
- There might be settings you want to start instances with that are not available in the launcher. You can now specify an arbitrary command-line argument in the settings file : this argument will simply be appended when the Launcher starts an instance. If you want to get rid of it, simply delete it from the settings file and restart the launcher for this to be taken into account.

## How to get it

Get the latest version in the [here](https://github.com/sebescudie/GammaLauncher/releases), or clone the repo and build the app yourself. If you do so, don't forget to copy the `ico.ico` file to your output dir, otherwise the app won't start.

## Contributing

If you wanna contribute to the app by adding functionalities or whatever, please do so!

cheerz
