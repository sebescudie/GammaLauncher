using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using System.IO;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.IO;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using _build;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Clean);

    const string TargetMagicString      = "##TARGET##";
    const string VersionMagicString     = "##VERSION##";
    const string winx64TargetString     = "win-x64";
    const string winArm64TargetString   = "win-arm64";

    AbsolutePath innoCompilerPath       = "C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe";

    AbsolutePath InnoFolder             = RootDirectory / .. / "inno";
    AbsolutePath InnoTemplate           = RootDirectory / .. / "inno/installer.iss.template";
    AbsolutePath InnoScript             = RootDirectory / .. / "inno/installer.iss";

    AbsolutePath NuspecFile             = RootDirectory / .. / "choco/GammaLauncher/gammalauncher.nuspec";
    AbsolutePath ChocoToolsFolder       = RootDirectory / .. / "choco/GammaLauncher/tools";


    AbsolutePath ArtifactsDirectory    = RootDirectory / .. / "artifacts";
    AbsolutePath VvvvPropsFile          = RootDirectory / .. / "GammaLauncher.props";
    AbsolutePath VvvvSourceFile         = RootDirectory / .. / "GammaLauncher.vl";

    string Version                      = "";
    AbsolutePath VersionFile            = RootDirectory / .. / "Version.props";

    [Parameter("API Key for Chocolatey feed")]
    readonly string ApiKey;

    [Parameter("Chocolatey feed URL")]
    readonly string Feed;

    Target Clean => _ => _
        .Executes(() =>
        {
            Console.WriteLine("Purging vvvv artifacts folder...");
            Utils.DeleteDirectoryContent(ArtifactsDirectory);
            
            // Delete installer from inno and choco/tools folders
            // We search in both folder in case something got wrong during previous run and the installer
            // was not moved to /tools
            var exeInTools = Directory.EnumerateFiles(ChocoToolsFolder).FirstOrDefault(f => Path.GetFileName(f).EndsWith("exe"));
            if(!exeInTools.IsNullOrEmpty())
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
            // Parse GammaLauncher entry point to find the vvvversion it was saved with
            // and infer compiler path from that
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(File.ReadAllText(VvvvSourceFile));
            var xmlVersion = xmlDoc.SelectSingleNode("//Document/@LanguageVersion").Value ?? throw new InvalidDataException("Could not parse vvvv source file");

            // For now we're hardcoding the architecture but should find something more elegant at some point
            var vvvvFolderName = $"vvvv_gamma_{xmlVersion.Remove(0, 5)}-win-x64";
            var compilerPath = Path.Combine("C:\\Program Files\\vvvv", vvvvFolderName,"vvvvc.exe");
            Console.WriteLine($"GammaLauncher was last saved with {xmlVersion}");
            Console.WriteLine($"Expecting to find compiler in {compilerPath}");
            if(File.Exists(compilerPath))
                Console.WriteLine("Found corresponding CLI compiler");

            // Set rid to win-x64
            var launcherPropsXdoc = XDocument.Load(VvvvPropsFile);

            launcherPropsXdoc.Descendants(XName.Get("RuntimeIdentifier", "http://schemas.microsoft.com/developer/msbuild/2003")).First().Value = winx64TargetString;
            launcherPropsXdoc.Save(VvvvPropsFile);

            // Compile win-x64
            var buildWinx64 = ProcessTasks.StartProcess(compilerPath, $"{VvvvSourceFile} --output-type WinExe --rid win-x64 --clean");
            buildWinx64.WaitForExit();

            // Set rid to win-arm64
            launcherPropsXdoc.Descendants(XName.Get("RuntimeIdentifier", "http://schemas.microsoft.com/developer/msbuild/2003")).First().Value = winArm64TargetString;
            launcherPropsXdoc.Save(VvvvPropsFile);

            // Compile win-arm
            var buildWinArm = ProcessTasks.StartProcess(compilerPath, $"{VvvvSourceFile} --output-type WinExe --rid win-arm64 --clean");
            buildWinArm.WaitForExit();

            // Delete src folders
            var winx64SrcFolder = ArtifactsDirectory / winx64TargetString / "src";
            if(Directory.Exists(winx64SrcFolder))
                Directory.Delete(winx64SrcFolder, true);

            var winarm64SrcFolder = ArtifactsDirectory / winArm64TargetString / "src";
            if(Directory.Exists(winarm64SrcFolder))
                Directory.Delete(winarm64SrcFolder, true);

            // Create portable zips
            var winx64BuildOutput = ArtifactsDirectory / winx64TargetString;
            winx64BuildOutput.ZipTo(ArtifactsDirectory / $"gammalauncher_{Version}_win-x64_portable.zip");

            var winarm64BuildOutput = ArtifactsDirectory / winArm64TargetString;
            winarm64BuildOutput.ZipTo(ArtifactsDirectory / $"gammalauncher_{Version}_win-arm64_portable.zip");
        });

    // Create installer
    Target BuildInstaller => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // Write version in template and render it for winx64
            var content = File.ReadAllText(InnoTemplate);
            content = content.Replace(VersionMagicString, Version)
                             .Replace(TargetMagicString, winx64TargetString);
            File.WriteAllText(InnoScript, content);

            // Build winx64 installer
            var winX64installerCompileProcess = ProcessTasks.StartProcess(innoCompilerPath, $"/DMyAppVersion={Version} /DMyTarget={winx64TargetString} {InnoScript}");
            winX64installerCompileProcess.WaitForExit();

            // Write version in template and render it for winArm64
            content = File.ReadAllText(InnoTemplate);
            content = content.Replace(VersionMagicString, Version)
                             .Replace(TargetMagicString, winArm64TargetString);
            File.WriteAllText(InnoScript, content);

            // Build arm64 installer
            var Arm64installerCompileProcess = ProcessTasks.StartProcess(innoCompilerPath, $"/DMyAppVersion={Version} /DMyTarget={winArm64TargetString} {InnoScript}");
            Arm64installerCompileProcess.WaitForExit();
        });

    // Create Chocolatey package
    Target Pack => _ => _
        .DependsOn(GetVersion)
        .DependsOn(BuildInstaller)
        .Executes(() =>
        {
            var nuspecXDoc = XDocument.Load(NuspecFile);
            nuspecXDoc.Descendants(XName.Get("version", "http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd")).FirstOrDefault().Value = Version;
            nuspecXDoc.Save(NuspecFile);

            AbsolutePath installerPath = ArtifactsDirectory / $"gammalauncher_{Version}_{winx64TargetString}_installer.exe";
            var installerTargetPath = $"{ChocoToolsFolder}/{Path.GetFileName(installerPath)}";
            File.Copy(installerPath, installerTargetPath);

            // Pack
            var packProcess = ProcessTasks.StartProcess("choco", $"pack {NuspecFile}", ArtifactsDirectory);
            packProcess.WaitForExit();
        });

    // Publish Chocolatey package
    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => !string.IsNullOrWhiteSpace(Feed))
        .Requires(() => !string.IsNullOrWhiteSpace(ApiKey))
        .Executes(() =>
        {
            // Locate nupkg, which should be in nuke's RootDirectory
            var nupkg = Directory.EnumerateFiles(RootDirectory).FirstOrDefault(f => Path.GetExtension(f) == ".nupkg");
            Console.WriteLine($"Nupkg: {nupkg}");
            if (!nupkg.IsNullOrEmpty())
            {
                var pushProcess = ProcessTasks.StartProcess("choco", $"push {nupkg} --source {Feed} --api-key {ApiKey}");
                pushProcess.WaitForExit();
            }
            else
            {
                Console.WriteLine("Could not file nupkg");
            }
        });
}