using _build;
using ICSharpCode.SharpZipLib.Checksum;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Chocolatey;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Octokit;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Clean);

    // =======================================================
    // PARAMS
    // =======================================================

    [Parameter("Compiler Path")]
    readonly string CompilerPath;

    [Parameter("API Key for Chocolatey feed")]
    [Secret]
    readonly string ApiKey;

    [Parameter("Chocolatey feed URL")]
    readonly string Feed;

    // =======================================================
    // PATHS AND MAGIC STRINGS
    // =======================================================
    
    string Version = "";
    
    const string winX64Rid = "win-x64";
    const string winArm64Rid = "win-arm64";

    AbsolutePath innoCompilerPath = "C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe";

    AbsolutePath InnoScript = RootDirectory / .. / "inno/installer.iss";

    AbsolutePath NuspecFile = RootDirectory / .. / "choco/GammaLauncher/gammalauncher.nuspec";
    AbsolutePath ChocoToolsFolder = RootDirectory / .. / "choco/GammaLauncher/tools";


    AbsolutePath ArtifactsDirectory = RootDirectory / .. / "artifacts";
    AbsolutePath VvvvPropsFile = RootDirectory / .. / "GammaLauncher.props";
    AbsolutePath VvvvSourceFile = RootDirectory / .. / "GammaLauncher.vl";

    AbsolutePath VersionFile = RootDirectory / .. / "Version.props";

    // =======================================================
    // RELEASE
    // =======================================================

    string GithubToken = Environment.GetEnvironmentVariable("GAMMALAUNCHER_GITHUB_TOKEN", EnvironmentVariableTarget.User);

    Target Clean => _ => _
        .Executes(() =>
        {
            Console.WriteLine("Purging vvvv artifacts folder...");
            Utils.DeleteDirectoryContent(ArtifactsDirectory);

            // Delete installer from inno and choco/tools folders
            // We search in both folder in case something got wrong during previous run and the installer
            // was not moved to /tools
            var exeInTools = Directory.EnumerateFiles(ChocoToolsFolder).FirstOrDefault(f => Path.GetFileName(f).EndsWith("exe"));
            if (!exeInTools.IsNullOrEmpty())
            {
                Console.WriteLine("Deleting outdated exe from Choco /tools folder");
                File.Delete(exeInTools);
            }
        });


    Target GetVersion => _ => _
       .Executes(() =>
       {
           try
           {
               Version = XDocument.Load(VersionFile).Descendants("Version").FirstOrDefault()?.Value ?? "0.0.0";
               Console.WriteLine($"Attempting to build version {Version}");
           }
           catch
           {
               Console.WriteLine($"Could not extract version from {VersionFile}, aborting");
               throw;
           }
       });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(GetVersion)
        .Executes(() =>
        {
            // Set rid to win-x64
            var launcherPropsXdoc = XDocument.Load(VvvvPropsFile);

            launcherPropsXdoc.Descendants(XName.Get("RuntimeIdentifier", "http://schemas.microsoft.com/developer/msbuild/2003")).First().Value = winX64Rid;
            launcherPropsXdoc.Save(VvvvPropsFile);

            // Compile win-x64
            var buildWinx64 = ProcessTasks.StartProcess(CompilerPath, $"{VvvvSourceFile} --output-type WinExe --rid {winX64Rid} --clean");
            buildWinx64.WaitForExit();

            // Set rid to win-arm64
            launcherPropsXdoc.Descendants(XName.Get("RuntimeIdentifier", "http://schemas.microsoft.com/developer/msbuild/2003")).First().Value = winArm64Rid;
            launcherPropsXdoc.Save(VvvvPropsFile);

            // Compile win-arm
            var buildWinArm = ProcessTasks.StartProcess(CompilerPath, $"{VvvvSourceFile} --output-type WinExe --rid {winArm64Rid} --clean");
            buildWinArm.WaitForExit();

            // Delete src folders
            var winx64SrcFolder = ArtifactsDirectory / winX64Rid / "src";
            if (Directory.Exists(winx64SrcFolder))
                Directory.Delete(winx64SrcFolder, true);

            var winarm64SrcFolder = ArtifactsDirectory / winArm64Rid / "src";
            if (Directory.Exists(winarm64SrcFolder))
                Directory.Delete(winarm64SrcFolder, true);

            // Create portable zips
            var winx64BuildOutput = ArtifactsDirectory / winX64Rid;
            winx64BuildOutput.ZipTo(ArtifactsDirectory / $"gammalauncher_{Version}_{winX64Rid}_portable.zip");

            var winarm64BuildOutput = ArtifactsDirectory / winArm64Rid;
            winarm64BuildOutput.ZipTo(ArtifactsDirectory / $"gammalauncher_{Version}_{winArm64Rid}_portable.zip");
        });

    // Create installer
    Target BuildInstallers => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // Build winx64 installer
            var winX64installerCompileProcess = ProcessTasks.StartProcess(innoCompilerPath, $"/DMyAppVersion={Version} /DMyTarget={winX64Rid} {InnoScript}");
            winX64installerCompileProcess.WaitForExit();

            // Build arm64 installer
            var Arm64installerCompileProcess = ProcessTasks.StartProcess(innoCompilerPath, $"/DMyAppVersion={Version} /DMyTarget={winArm64Rid} {InnoScript}");
            Arm64installerCompileProcess.WaitForExit();
        });

    // Create Github release
    Target CreateGithubRelease => _ => _
        .Requires(() => !string.IsNullOrWhiteSpace(GithubToken))
        .Executes(async () =>
        {
            // Create release
            GitHubClient client = new GitHubClient(new ProductHeaderValue("gammalauncher.nuke"));
            client.Credentials = new Credentials(GithubToken);

            var release = new NewRelease(Version);
            release.Name = Version;
            release.Body = "";

            var result = await client.Repository.Release.Create("sebescudie", "GammaLauncher", release);
      
            // Upload artifacts
            var lastRelease = await client.Repository.Release.GetLatest("sebescudie", "GammaLauncher");

            var artifacts = ArtifactsDirectory.GlobFiles("*.zip", "*.exe");

            foreach(var artifact in artifacts)
            {
                var releaseAsset = new ReleaseAssetUpload
                {
                    FileName = artifact.Name,
                    RawData = File.OpenRead(artifact)
                };
            }
        });

    // Create Chocolatey package
    //Target PackChocolatey => _ => _
    //    .DependsOn(CreateGithubRelease)
    //    .Executes(async () =>
    //    {
    //        // Fetch latest release
    //        GitHubClient client = new GitHubClient(new ProductHeaderValue("gammalauncher.nuke"));
    //        client.Credentials = new Credentials(GithubToken);

    //        var lastRelease = await client.Repository.Release.GetLatest("sebescudie", "GammaLauncher");

    //        // Generate chocoInstall.ps1
    //        var installScript = $@"
    //        $ErrorActionPreference = 'Stop'

    //        $packageArgs = @{{
    //            packageName    = 'gammalauncher'
    //            fileType       = 'exe'
    //            url64bit       = '{lastRelease.Url}'
    //            checksum64     = '{}'
    //            checksumType64 = 'sha256'
    //            silentArgs     = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-'
    //            validExitCodes = @(0)
    //        }}

    //        Install-ChocolateyPackage @packageArgs".TrimStart();

    //        // Pack
    //        ChocolateyTasks.ChocolateyPack(settings => settings
    //            .SetPathToNuspec(installerTargetPath)
    //            .SetOutputDirectory(ArtifactsDirectory)
    //            .SetVersion(Version));
    //    });

    //// Publish Chocolatey package
    //Target PublishChocolatey => _ => _
    //    .DependsOn(PackChocolatey)
    //    .Requires(() => !string.IsNullOrWhiteSpace(Feed))
    //    .Requires(() => !string.IsNullOrWhiteSpace(ApiKey))
    //    .Executes(() =>
    //    {
    //        ChocolateyTasks.ChocolateyPush(settings => settings
    //            .SetProcessWorkingDirectory(ArtifactsDirectory)
    //            .SetApiKey(ApiKey));
    //    });
}