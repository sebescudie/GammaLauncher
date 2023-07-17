## Changelog

### 5.2.0

_No stable released yet, preview version only_

#### New

- App can minimize to system tray, thanks [bj-rn](https://github.com/sebescudie/GammaLauncher/commit/072e043aa58f49867e9b83185e94b1d916c191db)!
- A new _App Settings_ section in the _Settings_ tab allows to set whether to enable that feature or not
- You can now enable or disable editable packages and repositories folders without deleting them. Kinda like a mute button. Thanks [mhusinsky](https://github.com/sebescudie/GammaLauncher/issues/57) for proposing that feature!
- vvvv versions installed via the launcher will now be installed to the directory specified in the _vvvv installation path_ setting, thanks again [bj-rn](https://github.com/sebescudie/GammaLauncher/pull/63)!

#### Changed

- The "Yep" button that monsterkills vvvv instances is now bigger and closer to the _Kill vvvv_ button so that you don't have to aim for it in panic situations. Thanks [fleischverpackung](https://github.com/fleischverpackung) for the suggestion!

### 5.1.0

#### New

- You can now specify "Editable packages" in the settings tab. Typing the name of a package here and starting vvvv will allow you to edit this package. Thanks to @gregsn for this one!

#### Fixed

- The launcher should now accept vvvv installations using the new version format
- When you uninstall a version, the launcher will select the latest available after the uninstaller has ran. This should get rid of the weird behavior where an uninstalled version would still be displayed in the dropdown
- Now shows a warning if a user tries to open the nuget folder without having selected a vvvversion first (which should only happen in vvvvery rare occasions)

#### Updated

- Installer exit code is now logged in the _Updates_ tab

### 5.0.0

#### New

- Rewrote UI using VL.ImGUI
- Commes with a super cool installer
- Settings are now handled with a GUI : no need to manually tweak an XML file anymore!
- Settings are applied live : no need to restart the launcher when updating them

#### Fixes

- Now retrieves vvvv 5.0 preview builds
- Faster startup times!

#### Droped

- No more system tray icon. With the move to .NET 6 I could not repatch this one, will tackle it in a future update

### 4.2.0

- The settings file gets a new `CustomArg` element where one can type an arbitrary command-line argument that will be used every time.

### 4.1.2

- If the path of `package-repositories` contained spaces, either vvvv could not be started from the launcher, or the _Repos_ button would not work. This is now fixed : if your package repositories folder contains spaces, you should write it between quotes in your settings files (#36)

### 4.1.1

- Elementa custom styles assignd to Create, resulting in a major performance improvement, thanks to  @bj-rn for this one! (#29)
- Fixes Kill VVVV overlay button not closing when being clicked

### 4.1.0

- When an uninstall succedes, a version scan is triggered so that the dropdown is always up to date
- Fixes drawing of the button pointing to new launche releases
- Now uses a custom icon

### 4.0.1

- Gamma version can be uninstalled with a simple click or via <kbd>CTRL</kbd> + <kbd>U</kbd>
- <kbd>ALT</kbd>+<kbd>E</kbd> will open the selected version's installation folder

### 4.0.0-beta01

- Now supports multiple `--package-repositories` in the settings file
- Settings are now saved in `%APPDATA%\local\GammaLauncher` so that they can persist between updates

### 3.5.3

- Now queries Teamcity API the same way visualprogramming.net does (thanks @antongit and @tebjan for the teamcity madness)
- Forces app to run @60FPS when running in the IDE

### 3.5.2

#### Fixed

- Gets rid of exception if the launcher is started with no internet connection (reported by dottore)

### 3.5.1

#### New

- Massive performance improvement thanks to @gregsn (massive thanks to him) : the app now runs slooooowly when not focused. No more CPU madness!

- A burger menu on the left checks for new gamma versions and installs them. Also tells you if a newer GammaLauncher is available

- Drag and drop a `.vl` document on the launcher to open it in a new vvvv instance (if you have Allow Multiple enabled)

- Pressing <kbd>CTRL</kbd>+<kbd>R</kbd> also checks if newer builds are available online

#### Fixed

- Clicking on nuget folder when a stable version is selected opens the stable gamma's nuget folder. When a preview is selected, it opens preview's nuget folder (asked by motzi)
- Remembers previous window position on startup (asked by dottore)

#### Changed

- Reversed versions order in the dropdown menu. Newer versions are on top (asked by mburk)
