using _build;
using ICSharpCode.SharpZipLib.Checksum;
using Fallout.Common;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.Chocolatey;
using Fallout.Common.Tools.GitVersion;
using Fallout.Common.Utilities.Collections;
using Octokit;
using System;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Text;

class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.Clean);

    // =======================================================
    // PARAMS
    // =======================================================

    [Parameter("Compiler Path")]
    readonly string CompilerPath;
    
    [Parameter("Chocolatey feed URL")]
    readonly string Feed = "https://push.chocolatey.org/";

    // =======================================================
    // PATHS AND MAGIC STRINGS
    // =======================================================
    
    string Version => XDocument.Load(VersionFile).Descendants("Version").FirstOrDefault()?.Value ?? "0.0.0";
    
    const string WinX64Rid = "win-x64";
    const string WinArm64Rid = "win-arm64";

    readonly AbsolutePath InnoCompilerPath = "C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe";

    readonly AbsolutePath InnoScript = RootDirectory / .. / "inno/installer.iss";

    readonly AbsolutePath NuspecFile = RootDirectory / .. / "choco/GammaLauncher/gammalauncher.nuspec";
    readonly AbsolutePath ChocoToolsFolder = RootDirectory / .. / "choco/GammaLauncher/tools";
    
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / .. / "artifacts";
    readonly AbsolutePath VvvvPropsFile = RootDirectory / .. / "GammaLauncher.props";
    readonly AbsolutePath VvvvSourceFile = RootDirectory / .. / "GammaLauncher.vl";

    readonly AbsolutePath VersionFile = RootDirectory / .. / "Version.props";
    
    // =======================================================
    // RELEASE
    // =======================================================

    readonly string GithubToken = Environment.GetEnvironmentVariable("GAMMALAUNCHER_GITHUB_TOKEN", EnvironmentVariableTarget.User);
    readonly string ChocolateyApiKey = Environment.GetEnvironmentVariable("GAMMALAUNCHER_CHOCOLATEY_API_KEY", EnvironmentVariableTarget.User);
    
    Target Clean => _ => _
        .Executes(() =>
        {
            Log.Information("Deleting outdated executables from Chocolatey /tools folder");
            ChocoToolsFolder.GlobFiles("*.exe").DeleteFiles();
        });
    
    Target Compile => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            // Set rid to win-x64
            var launcherPropsXdoc = XDocument.Load(VvvvPropsFile);

            launcherPropsXdoc.Descendants(XName.Get("RuntimeIdentifier", "http://schemas.microsoft.com/developer/msbuild/2003")).First().Value = WinX64Rid;
            launcherPropsXdoc.Save(VvvvPropsFile);

            // Compile win-x64
            var buildWinx64 = ProcessTasks.StartProcess(CompilerPath, $"{VvvvSourceFile} --output-type WinExe --rid {WinX64Rid} --clean");
            buildWinx64.WaitForExit();

            // Set rid to win-arm64
            launcherPropsXdoc.Descendants(XName.Get("RuntimeIdentifier", "http://schemas.microsoft.com/developer/msbuild/2003")).First().Value = WinArm64Rid;
            launcherPropsXdoc.Save(VvvvPropsFile);

            // Compile win-arm
            var buildWinArm = ProcessTasks.StartProcess(CompilerPath, $"{VvvvSourceFile} --output-type WinExe --rid {WinArm64Rid} --clean");
            buildWinArm.WaitForExit();

            // Delete src folders
            var winx64SrcFolder = ArtifactsDirectory / WinX64Rid / "src";
            if (Directory.Exists(winx64SrcFolder))
                Directory.Delete(winx64SrcFolder, true);

            var winarm64SrcFolder = ArtifactsDirectory / WinArm64Rid / "src";
            if (Directory.Exists(winarm64SrcFolder))
                Directory.Delete(winarm64SrcFolder, true);

            // Create portable zips
            var winx64BuildOutput = ArtifactsDirectory / WinX64Rid;
            winx64BuildOutput.ZipTo(ArtifactsDirectory / $"gammalauncher_{Version}_{WinX64Rid}_portable.zip");

            var winarm64BuildOutput = ArtifactsDirectory / WinArm64Rid;
            winarm64BuildOutput.ZipTo(ArtifactsDirectory / $"gammalauncher_{Version}_{WinArm64Rid}_portable.zip");
        });

    // Create installer
    Target BuildInstallers => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // Build winx64 installer
            var winX64InstallerCompileProcess = ProcessTasks.StartProcess(InnoCompilerPath, $"/DMyAppVersion={Version} /DMyTarget={WinX64Rid} {InnoScript}");
            winX64InstallerCompileProcess.WaitForExit();

            // Build arm64 installer
            var arm64InstallerCompileProcess = ProcessTasks.StartProcess(InnoCompilerPath, $"/DMyAppVersion={Version} /DMyTarget={WinArm64Rid} {InnoScript}");
            arm64InstallerCompileProcess.WaitForExit();
        });

    // Create GitHub release
    Target CreateGithubRelease => _ => _
        .Requires(() => !string.IsNullOrWhiteSpace(GithubToken))
        .Executes(async () =>
        {
            // Create release
            var client = new GitHubClient(new ProductHeaderValue("gammalauncher.fallout"))
            {
                Credentials = new Credentials(GithubToken)
            };

            var release = new NewRelease(Version)
            {
                Name = Version,
                Body = ""
            };

            var result = await client.Repository.Release.Create("sebescudie", "GammaLauncher", release);
      
            // Upload artifacts
            var lastRelease = await client.Repository.Release.GetLatest("sebescudie", "GammaLauncher");

            var artifacts = ArtifactsDirectory.GlobFiles("*.zip", "*.exe");

            foreach(var artifact in artifacts)
            {
                var releaseAsset = new ReleaseAssetUpload
                {
                    FileName = artifact.Name,
                    RawData = File.OpenRead(artifact),
                    ContentType = "application/octet-stream"
                };
                Log.Information($"Uploading {releaseAsset.FileName}");
                var upload = await client.Repository.Release.UploadAsset(lastRelease, releaseAsset);
            }
        });

    //Create Chocolatey package
    Target PackChocolatey => _ => _
        .Executes(async () =>
        {
            // Fetch latest release and win64 installer asset
            GitHubClient client = new GitHubClient(new ProductHeaderValue("gammalauncher.nuke"));
            client.Credentials = new Credentials(GithubToken);

            var lastRelease = await client.Repository.Release.GetLatest("sebescudie", "GammaLauncher");

            var win64InstallerReleaseAsset = lastRelease.Assets.FirstOrDefault(asset => asset.Name.Contains("win-x64_installer.exe"))
                ?? throw new InvalidOperationException($"Could not find win64 installer asset in release {lastRelease}");

            // Calculate SHA256 manually since Octokit does not return it
            var winX64InstallerFile = ArtifactsDirectory.GetFiles().FirstOrDefault(file => file.Name.Contains("win-x64_installer.exe"))
                ?? throw new FileNotFoundException($"Could not locate win64 installer in {ArtifactsDirectory}");
            
            var winX64InstallerHash = "";
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(File.ReadAllBytes(winX64InstallerFile));
                winX64InstallerHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            
            // Generate chocoInstall.ps1
            var installScript = $@"
$ErrorActionPreference = 'Stop'

$packageArgs = @{{
packageName    = 'gammalauncher'
fileType       = 'exe'
url64bit       = '{win64InstallerReleaseAsset.BrowserDownloadUrl}'
checksum64     = '{winX64InstallerHash}'
checksumType64 = 'sha256'
silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
validExitCodes = @(0)
}}

Install-ChocolateyPackage @packageArgs".TrimStart();

            File.WriteAllText(ChocoToolsFolder / "chocolateyinstall.ps1", installScript);

            // Pack
            ChocolateyTasks.ChocolateyPack(settings => settings
            .SetPathToNuspec(NuspecFile)
            .SetOutputDirectory(ArtifactsDirectory)
            .SetVersion(Version));
        });

    // Publish Chocolatey package
    Target PublishChocolatey => _ => _
        .DependsOn(PackChocolatey)
        .Requires(() => !string.IsNullOrWhiteSpace(Feed))
        .Requires(() => !string.IsNullOrWhiteSpace(ChocolateyApiKey))
        .Executes(() =>
        {
            ChocolateyTasks.ChocolateyPush(settings => settings
                .SetProcessWorkingDirectory(ArtifactsDirectory)
                .SetSource(Feed)
                .SetApiKey(ChocolateyApiKey));
        });
}