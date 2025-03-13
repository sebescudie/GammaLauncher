using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Version")]
    readonly string Version = "1.0.0";

    [Parameter("API Key for Chocolatey feed")]
    readonly string ChocoApiKey;

    [Parameter("Chocolatey feed URL")]
    readonly string ChocoFeed;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .Executes(() =>
        {
        });

    // Patch is built manually so we're leaving that empty
    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
        });

    // Create installer
    Target BuildInstaller => _ => _
        .Executes(() =>
        {
            var innoScriptTemplate = RootDirectory / "inno/installer.iss.template";
            var innoScript = RootDirectory / "inno/installer.iss";

            // Write version in template and render it
            var content = File.ReadAllText(innoScriptTemplate);
            content = content.Replace("##VERSION##", Version);
            File.WriteAllText(innoScript, content);

            var innoCompilerPath = "C:\\Program Files (x86)\\Inno Setup 6\\ISCC.exe";
            var installerCompileProcess = ProcessTasks.StartProcess(innoCompilerPath, innoScript);
            installerCompileProcess.WaitForExit();
        });

    // Create Chocolatey package
    Target PackageChocolatey => _ => _
        .Executes(() =>
        {

        });

    // Publish Chocolatey package
    Target PublishChocolatey => _ => _
        .Executes(() =>
        {

        });

}
