using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common;
using ILRepacking;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.SignTool;
using Nuke.Common.Utilities.Collections;
using Nuke.OctoVersion;
using OctoVersion.Core;
using static Nuke.Common.Logger;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SignTool.SignToolTasks;

[VerbosityMapping(typeof(DotNetVerbosity),
    Verbose = nameof(DotNetVerbosity.Diagnostic))]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Default);
    //////////////////////////////////////////////////////////////////////
    // ARGUMENTS
    //////////////////////////////////////////////////////////////////////
    [Parameter] readonly string Configuration = "Release";
    [Parameter] readonly string SigningCertificatePath = "./certificates/OctopusDevelopment.pfx";
    [Parameter] readonly string SigningCertificatePassword = "Password01!";
    ///////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ///////////////////////////////////////////////////////////////////////////////
    AbsolutePath PublishDir => RootDirectory / "publish";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath LocalPackagesDir => RootDirectory / ".." / "LocalPackages";
    AbsolutePath SourceDir => RootDirectory / "source";
    AbsolutePath OctopusClientFolder => SourceDir / "Octopus.Client";
    
    [NukeOctoVersion] readonly OctoVersionInfo OctoVersionInfo;

    Target Clean => _ => _
        .Executes(() =>
    {
        EnsureCleanDirectory(ArtifactsDir);
        EnsureCleanDirectory(PublishDir);
        SourceDir.GlobDirectories("**/bin").ForEach(x => EnsureCleanDirectory(x));
        SourceDir.GlobDirectories("**/obj").ForEach(x => EnsureCleanDirectory(x));
        SourceDir.GlobDirectories("**/TestResults").ForEach(x => EnsureCleanDirectory(x));
    });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(SourceDir)
                .SetVersion(OctoVersionInfo.FullSemVer));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(Clean)
        .Executes(() =>
    {
        DotNetBuild(_ => _
            .SetProjectFile(SourceDir)
            .SetConfiguration(Configuration)
            .SetVersion(OctoVersionInfo.FullSemVer));
    });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        RootDirectory.GlobFiles("**/**/*.Tests.csproj").ForEach(testProjectFile =>
        {
            DotNetTest(_ => _
                .SetProjectFile(testProjectFile)
                .SetConfiguration(Configuration)
                .SetNoBuild(true));
        });
    });

    Target Merge => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            var inputFolder = OctopusClientFolder / "bin" / Configuration / "net452";
            var outputFolder = OctopusClientFolder / "bin" / Configuration / "net452Merged";
            EnsureExistingDirectory(outputFolder);
            
            var assemblyPaths = System.IO.Directory.EnumerateFiles(inputFolder, "NewtonSoft.Json.dll").Select(f => (AbsolutePath)f);
            assemblyPaths = assemblyPaths.Concat(System.IO.Directory.EnumerateFiles(inputFolder, "Octodiff.exe").Select(f => (AbsolutePath)f));
            
            var inputAssemblies = new List<string> { $"{inputFolder}/Octopus.Client.dll" };
            inputAssemblies.AddRange(assemblyPaths.Select(x => x.ToString()));

            var repackSettings = new RepackOptions()
            {
                OutputFile = outputFolder / "Octopus.Client.dll",
                InputAssemblies = inputAssemblies.ToArray(),
                Internalize = true,
                Parallel = false,
                XmlDocumentation = true,
                SearchDirectories = new [] { inputFolder.ToString() }
            };
            
            new ILRepack(repackSettings).Repack();

            DeleteDirectory(inputFolder);
            MoveDirectory(outputFolder, inputFolder);
    });

    Target PackClientNuget => _ => _
        .DependsOn(Merge)
        .Executes(() =>
    {
        SignBinaries(OctopusClientFolder / "bin" / Configuration);
        try
        {
            ReplaceTextInFiles(OctopusClientFolder / "Octopus.Client.nuspec", "<version>$version$</version>",
                $"<version>{OctoVersionInfo.FullSemVer}</version>");
            
            DotNetPack(_ => _
            .SetProject(OctopusClientFolder)
            .SetProcessArgumentConfigurator(args =>
            {
                args.Add($"/p:NuspecFile=Octopus.Client.nuspec");
                return args;
            })
            .SetVersion(OctoVersionInfo.FullSemVer)
            .SetConfiguration(Configuration)
            .SetOutputDirectory(ArtifactsDir)
            .SetNoBuild(true)
            .SetIncludeSymbols(false)
            .SetVerbosity(DotNetVerbosity.Normal));
        }
        finally
        {
            ReplaceTextInFiles(OctopusClientFolder / "Octopus.Client.nuspec", $"<version>{OctoVersionInfo.FullSemVer}</version>", $"<version>$version$</version>");
        }
    });

    private void ReplaceTextInFiles(AbsolutePath path, string oldValue, string newValue)
    {
        string fileText = File.ReadAllText(path);
        fileText = fileText.Replace(oldValue, newValue);
        File.WriteAllText(path, fileText);
    }

    Target TestClientNugetPackage => _ => _
        .DependsOn(PackClientNuget)
        .Executes(() =>
    {
        // tests that make sure the packed, ilmerged dll we're going to ship actually works the way we expect it to
        DotNetTest(_ => _
            .SetProjectFile(SourceDir / "Octopus.Client.E2ETests" / "Octopus.Client.E2ETests.csproj")
            .SetConfiguration(Configuration)
            .SetNoBuild(true));
    });

    Target CopyToLocalPackages => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .DependsOn(TestClientNugetPackage)
        .Executes(() =>
    {
        EnsureExistingDirectory(LocalPackagesDir);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Client.{OctoVersionInfo.FullSemVer}.nupkg", LocalPackagesDir);
    });
    
    private void SignBinaries(AbsolutePath path)
    {
        Info($"Signing binaries in {path}");
        var files = path.GlobDirectories("**").SelectMany(x => x.GlobFiles("Octopus.*.dll"));

        SignTool(_ => _
            .SetFile(SigningCertificatePath)
            .SetPassword(SigningCertificatePassword)
            .SetFiles(files.Select(x => x.ToString()).ToArray())
            .SetProcessToolPath(RootDirectory / "certificates" / "signtool.exe")
            .SetTimestampServerDigestAlgorithm("sha256")
            .SetRfc3161TimestampServerUrl("http://rfc3161timestamp.globalsign.com/advanced"));
    }


    Target Default => _ => _
        .DependsOn(CopyToLocalPackages);
}
