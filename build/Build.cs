using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ILRepack;
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
    public static int Main() => Execute<Build>(x => x.TestClientNugetPackage);
    //////////////////////////////////////////////////////////////////////
    // ARGUMENTS
    //////////////////////////////////////////////////////////////////////
    [Parameter] readonly string Configuration = "Release";
    [Parameter] readonly string SigningCertificatePath = RootDirectory / "certificates" / "OctopusDevelopment.pfx";
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

    // Keep this list in order by most likely to succeed
    string[] SigningTimestampUrls => new [] {
        "http://tsa.starfieldtech.com",
        "http://www.startssl.com/timestamp",
        "http://timestamp.comodoca.com/rfc3161",
        "http://timestamp.verisign.com/scripts/timstamp.dll",
        "http://timestamp.globalsign.com/scripts/timestamp.dll",
        "https://rfc3161timestamp.globalsign.com/advanced"
    };

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
            foreach (var target in new [] {"net452", "netstandard2.0"})
            {
                var inputFolder = OctopusClientFolder / "bin" / Configuration / target;
                var outputFolder = OctopusClientFolder / "bin" / Configuration / $"{target}Merged";
                EnsureExistingDirectory(outputFolder);

                var assemblyPaths = inputFolder.GlobFiles("NewtonSoft.Json.dll", "Octodiff.*");

                var inputAssemblies = new List<string> { inputFolder / "Octopus.Client.dll" };
                inputAssemblies.AddRange(assemblyPaths.Select(x => x.ToString()));

                ILRepackTasks.ILRepack(_ => _
                    .SetAssemblies(inputAssemblies)
                    .SetOutput(outputFolder / "Octopus.Client.dll")
                    .EnableInternalize()
                    .DisableParallel()
                    .EnableXmldocs()
                    .SetLib(inputFolder)
                );

                DeleteDirectory(inputFolder);
                MoveDirectory(outputFolder, inputFolder);
            }
        });

    Target PackClientNuget => _ => _
        .DependsOn(Merge)
        .Executes(() =>
    {
        SignBinaries(OctopusClientFolder / "bin" / Configuration);
        var octopusClientNuspec = OctopusClientFolder / "Octopus.Client.nuspec";
        try
        {
            ReplaceTextInFiles(octopusClientNuspec, "<version>$version$</version>",
                $"<version>{OctoVersionInfo.FullSemVer}</version>");

            //ensure our dependencies here match the versions in the csproj
            foreach(var dependency in new []{ "Octopus.TinyTypes", "Octopus.TinyTypes.Json", "Octopus.TinyTypes.TypeConverters" })
            {
                var expectedVersion = XmlTasks.XmlPeek(OctopusClientFolder / "Octopus.Client.csproj", $"//Project/ItemGroup/PackageReference[@Include='{dependency}']/@Version").FirstOrDefault();
                XmlTasks.XmlPoke(octopusClientNuspec,$"//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETFramework4.5.2']/ns:dependency[@id='{dependency}']/@version", expectedVersion, ("ns", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd"));
                XmlTasks.XmlPoke(octopusClientNuspec,$"//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='{dependency}']/@version", expectedVersion, ("ns", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd"));
            }

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
                .EnableNoBuild()
                .DisableIncludeSymbols()
                .SetVerbosity(DotNetVerbosity.Normal));
        }
        finally
        {
            ReplaceTextInFiles(octopusClientNuspec, $"<version>{OctoVersionInfo.FullSemVer}</version>", $"<version>$version$</version>");
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
        .TriggeredBy(TestClientNugetPackage)
        .Executes(() =>
    {
        EnsureExistingDirectory(LocalPackagesDir);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Client.{OctoVersionInfo.FullSemVer}.nupkg", LocalPackagesDir);
    });

    private void SignBinaries(AbsolutePath path)
    {
        Info($"Signing binaries in {path}");
        var files = path.GlobDirectories("**").SelectMany(x => x.GlobFiles("Octopus.*.dll"));

        var lastException = default(Exception);
        foreach (var url in SigningTimestampUrls)
        {
            try
            {
                SignTool(_ => _
                    .SetFile(SigningCertificatePath)
                    .SetPassword(SigningCertificatePassword)
                    .SetFiles(files.Select(x => x.ToString()).ToArray())
                    .SetProcessToolPath(RootDirectory / "certificates" / "signtool.exe")
                    .SetTimestampServerDigestAlgorithm("sha256")
                    .SetDescription("Octopus Client Tool")
                    .SetUrl("https://octopus.com")
                    .SetRfc3161TimestampServerUrl(url));
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        }

        if(lastException != null)
            throw(lastException);
    }
}