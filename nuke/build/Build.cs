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

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Clean);

    string VersionMagicString = "##VERSION##";

    string InnoFolder = RootDirectory / .. / "inno";
    string InnoTemplate= RootDirectory / .. / "inno/installer.iss.template";
    string InnoScript = RootDirectory / .. / "inno/installer.iss";

    string NuspecTemplate = RootDirectory / .. / "choco/GammaLauncher/gammalauncher.nuspec.template";
    string NuspecFile = RootDirectory / .. / "choco/GammaLauncher/gammalauncher.nuspec";
    string ChocoToolsFolder = RootDirectory / .. / "choco/GammaLauncher/tools";
    string ChocoFolder = RootDirectory /.. / "choco";
    
    string VvvvOutputDirectory = RootDirectory / .. / "output";
    string VvvvPropsTemplate = RootDirectory / .. / "GammaLauncher.props.template";
    string VvvvPropsFile = RootDirectory / .. /"GammaLauncher.props";
    string VvvvSourceFile = RootDirectory / .. /"GammaLauncher.vl";

    [Parameter("Version")]
    readonly string Version = "";

    [Parameter("API Key for Chocolatey feed")]
    readonly string ApiKey;

    [Parameter("Chocolatey feed URL")]
    readonly string Feed;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            // Clean vvvv output folder
            if(Path.Exists(VvvvOutputDirectory))
                Directory.Delete(VvvvOutputDirectory, true);

            // Delete installer from inno and choco/tools folders
            // We search in both folder in case something got wrong during previous run and the installer
            // was not moved to /tools
            var exeInTools = Directory.EnumerateFiles(ChocoToolsFolder).FirstOrDefault(f => Path.GetFileName(f).EndsWith("exe"));
            if(!exeInTools.IsNullOrEmpty())
                File.Delete(exeInTools);

            var exeInInno = Directory.EnumerateFiles(InnoFolder).FirstOrDefault(f => Path.GetFileName(f).EndsWith("exe"));
            if(!exeInInno.IsNullOrEmpty())
                File.Delete(exeInInno);

            // Delete generated inno script
            if(File.Exists(InnoScript))
                File.Delete(InnoScript);

            // Delete generated nuspec
            if(File.Exists(NuspecFile))
                File.Delete(NuspecFile);

            // Delete generated props file
            if(File.Exists(VvvvPropsFile))
                File.Delete(VvvvPropsFile);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Console.WriteLine("Nothing to restore");
        });

    
    Target Compile => _ => _
        .DependsOn(Restore)
        .Requires(() => !string.IsNullOrWhiteSpace(Version))
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

            // Generate props file from template
            var content = File.ReadAllText(VvvvPropsTemplate);
            content = content.Replace(VersionMagicString, Version);
            File.WriteAllText(VvvvPropsFile, content);

            //Compile patch
            var compilationProcess = ProcessTasks.StartProcess(compilerPath, $"{VvvvSourceFile} --clean");
            compilationProcess.WaitForExit();
        });

    // Create installer
    Target BuildInstaller => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            // Write version in template and render it
            var content = File.ReadAllText(InnoTemplate);
            content = content.Replace(VersionMagicString, Version);
            File.WriteAllText(InnoScript, content);

            var innoCompilerPath = "C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe";
            var installerCompileProcess = ProcessTasks.StartProcess(innoCompilerPath, InnoScript);
            installerCompileProcess.WaitForExit();
        });

    // Create Chocolatey package
    Target Pack => _ => _
        .DependsOn(BuildInstaller)
        .Executes(() =>
        {
            // Write nuspec template and render it
            var content = File.ReadAllText(NuspecTemplate);
            content = content.Replace("##VERSION##", Version);
            File.WriteAllText(NuspecFile, content);

            // Retrieve installer that was built in BuildInstaller step
            // and move it to choco tool folder
            string pattern = @"^gammalauncher_.*_installer\.exe$";
            var installerPath = Directory.EnumerateFiles(InnoFolder).FirstOrDefault(f => Regex.IsMatch(Path.GetFileName(f), pattern));
            var installerTargetPath = $"{ChocoToolsFolder}/{Path.GetFileName(installerPath)}";
            File.Copy(installerPath, installerTargetPath);

            // Pack
            var packProcess = ProcessTasks.StartProcess("choco", $"pack {NuspecFile}");
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