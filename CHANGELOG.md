## Changelog

### 5.3.0

#### New

- A _Show Package Cache_ entry in the _Edit_ menu shows the vvvv package cache, thanks @bj-rn!
- A new button allows to open a `cmd` window in the selected vvvversion
- The installer now takes care of installing the .NET runtime for you
- A new _Export bat file_ in the _Tools_ menu allows to generate a bat file with the current launcher settings, thanks @lasalillo and @seltzdesign for asking!
- Now allows to download win-arm64 builds
- The launcher now exists in win-arm64 flavor, thanks @azeno for fixing the `vvvvc` command thing!
- A Log window can now be open from the _Tools_ menu
- The _Updates_ tab now shows an in-app changelog for every remote build, thanks @berkut0 for the request!
- Now shows a progress bar when downloading a build
- A new startup option allows to start vvvv as admin

#### Changed

- Start and uninstall buttons are now icons instead of text, thanks @joreg for requesting!
- Updates tab now shows a `*` when a new build is available instead of coloring the tab yellow
- The _Kill_ button now enumerates processes, looks for vvvv and kills them instead of just making a taskkill. Thanks @seltzdesign for the report!
- Reworked _Updates_ tab UI
- The version dropdown now shows the architecture next to the version number

#### Fixed

- Now checks whether all `package-repositories` folder exist before attempting to start vvvv. Was making it crash, thanks @TremensS for the report!

### 5.2.0

#### New

- You can now install stable and preview vvvversions from the launcher! ([#32](https://github.com/sebescudie/GammaLauncher/issues/32), [#58](https://github.com/sebescudie/GammaLauncher/issues/58))
- App can minimize to system tray. A new _App Settings_ section in the _Settings_ allows to enable or disable this feature ([#59](https://github.com/sebescudie/GammaLauncher/issues/59) by [bj-rn](https://github.com/bj-rn))
- You can now enable or disable editable packages and repositories folders without deleting them. Kinda like a mute button ([#57](https://github.com/sebescudie/GammaLauncher/issues/57))
- vvvv versions installed via the launcher will now be installed to the directory specified in the _vvvv installation path_ setting ([#63](https://github.com/sebescudie/GammaLauncher/issues/63))
- The launcher can now be installed via `winget`! Type `winget install GammaLauncher` and enjoy ([#12](https://github.com/sebescudie/GammaLauncher/issues/12), [#66](https://github.com/sebescudie/GammaLauncher/issues/66))

#### Changed

- The "Yep" button that monsterkills vvvv instances is now bigger and closer to the _Kill vvvv_ button so that you don't have to aim for it in panic situations ([#50](https://github.com/sebescudie/GammaLauncher/issues/50))
- Hovering startup options shows the correspondant startup flag ([#61](https://github.com/sebescudie/GammaLauncher/issues/61))

#### Fixed

- The launcher now discards failed vvvv builds in the Updates tab ([#67](https://github.com/sebescudie/GammaLauncher/issues/67))
- Fixes handling of paths with spaces when opening a document ([#60](https://github.com/sebescudie/GammaLauncher/issues/60))

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
