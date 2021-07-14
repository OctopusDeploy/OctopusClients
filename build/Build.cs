using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ILRepack;
using Nuke.Common.Tools.SignTool;
using Nuke.Common.Utilities.Collections;
using Nuke.OctoVersion;
using OctoVersion.Core;
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
    [Parameter] readonly string SigningCertificatePath = RootDirectory / "certificates" / "OctopusDevelopment.pfx";
    [Parameter] readonly string SigningCertificatePassword = "Password01!";

    [Parameter] string AzureKeyVaultUrl = "";
    [Parameter] string AzureKeyVaultAppId = "";
    [Parameter, Secret] string AzureKeyVaultAppSecret = "";
    [Parameter] string AzureKeyVaultCertificateName = "";
    ///////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ///////////////////////////////////////////////////////////////////////////////
    AbsolutePath PublishDir => RootDirectory / "publish";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath LocalPackagesDir => RootDirectory / ".." / "LocalPackages";
    AbsolutePath SourceDir => RootDirectory / "source";
    AbsolutePath OctopusClientFolder => SourceDir / "Octopus.Client";
    AbsolutePath OctopusNormalClientFolder => SourceDir / "Octopus.Server.Client";

    [NukeOctoVersion] readonly OctoVersionInfo OctoVersionInfo;

    [PackageExecutable(
        packageId: "azuresigntool",
        packageExecutable: "azuresigntool.dll")]
    readonly Tool AzureSignTool = null!;

    // Keep this list in order by most likely to succeed
    string[] SigningTimestampUrls => new[] {
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
            foreach (var target in new[] { "net452", "netstandard2.0" })
            {
                var inputFolder = OctopusClientFolder / "bin" / Configuration / target;
                var outputFolder = OctopusClientFolder / "bin" / Configuration / $"{target}Merged";
                EnsureExistingDirectory(outputFolder);

                // The call to ILRepack with .EnableInternalize() requires the Octopus.Server.Client.dll assembly to be first in the list.
                var inputAssemblies = inputFolder.GlobFiles("NewtonSoft.Json.dll", "Octodiff*", "Octopus.*.dll")
                    .Select(x => x.ToString())
                    .OrderByDescending(x => x.Contains("Octopus.Server.Client.dll"))
                    .ThenBy(x => x)
                    .ToArray();

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

    Target PackMergedClientNuget => _ => _
        .DependsOn(Merge)
        .Executes(() =>
    {
        SignBinaries(OctopusClientFolder / "bin" / Configuration);
        var octopusClientNuspec = OctopusClientFolder / "Octopus.Client.nuspec";
        try
        {
            ReplaceTextInFiles(octopusClientNuspec, "<version>$version$</version>",
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
                .EnableNoBuild()
                .DisableIncludeSymbols()
                .SetVerbosity(DotNetVerbosity.Normal));
        }
        finally
        {
            ReplaceTextInFiles(octopusClientNuspec, $"<version>{OctoVersionInfo.FullSemVer}</version>", $"<version>$version$</version>");
        }
    });

    Target PackNormalClientNuget => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        SignBinaries(OctopusNormalClientFolder / "bin" / Configuration);

        DotNetPack(_ => _
            .SetProject(OctopusNormalClientFolder)
            .SetVersion(OctoVersionInfo.FullSemVer)
            .SetConfiguration(Configuration)
            .SetOutputDirectory(ArtifactsDir)
            .EnableNoBuild()
            .DisableIncludeSymbols()
            .SetVerbosity(DotNetVerbosity.Normal));
    });

    Target TestClientNugetPackage => _ => _
        .DependsOn(PackMergedClientNuget)
        .Executes(() =>
    {
        // Tests that make sure the packed, ILMerged DLL we're going to ship actually works the way we expect it to.
        DotNetTest(_ => _
            .SetProjectFile(SourceDir / "Octopus.Client.E2ETests" / "Octopus.Client.E2ETests.csproj")
            .SetConfiguration(Configuration)
            .SetNoBuild(true));
    });

    Target CopyToLocalPackages => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .DependsOn(PackNormalClientNuget)
        .DependsOn(PackMergedClientNuget)
        .Executes(() =>
    {
        EnsureExistingDirectory(LocalPackagesDir);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Client.{OctoVersionInfo.FullSemVer}.nupkg", LocalPackagesDir, FileExistsPolicy.Overwrite);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Server.Client.{OctoVersionInfo.FullSemVer}.nupkg", LocalPackagesDir, FileExistsPolicy.Overwrite);
    });

    Target Default => _ => _
        .DependsOn(CopyToLocalPackages)
        .DependsOn(PackNormalClientNuget)
        .DependsOn(PackMergedClientNuget)
        .DependsOn(TestClientNugetPackage);

    void SignBinaries(AbsolutePath path)
    {
        Logger.Info($"Signing binaries in {path}");
        var files = path.GlobDirectories("**").SelectMany(x => x.GlobFiles("Octopus.*.dll")).ToArray();

        var useSignTool = string.IsNullOrEmpty(AzureKeyVaultUrl)
                          && string.IsNullOrEmpty(AzureKeyVaultAppId)
                          && string.IsNullOrEmpty(AzureKeyVaultAppSecret)
                          && string.IsNullOrEmpty(AzureKeyVaultCertificateName);
        var lastException = default(Exception);
        foreach (var url in SigningTimestampUrls)
        {
            TeamCity.Instance?.OpenBlock("Signing and timestamping with server " + url);
            try
            {
                if (useSignTool)
                    SignWithSignTool(files, url);
                else
                    SignWithAzureSignTool(files, url);
                lastException = null;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
            TeamCity.Instance?.CloseBlock("Signing and timestamping with server " + url);
            if (lastException == null)
                break;
        }

        if (lastException != null)
            throw lastException;
        Logger.Info($"Finished signing {files.Length} files.");
    }

    void SignWithAzureSignTool(AbsolutePath[] files, string timestampUrl)
    {
        Logger.Info("Signing files using azuresigntool and the production code signing certificate.");

        var arguments = "sign " +
                        $"--azure-key-vault-url \"{AzureKeyVaultUrl}\" " +
                        $"--azure-key-vault-client-id \"{AzureKeyVaultAppId}\" " +
                        $"--azure-key-vault-client-secret \"{AzureKeyVaultAppSecret}\" " +
                        $"--azure-key-vault-certificate \"{AzureKeyVaultCertificateName}\" " +
                        $"--file-digest sha256 " +
                        $"--description \"Octopus Client Library\" " +
                        $"--description-url \"https://octopus.com\" " +
                        $"--timestamp-digest sha256 ";
                        $"--timestamp-rfc3161 {timestampUrl} " +

        foreach (var file in files)
            arguments += $"\"{file}\" ";

        AzureSignTool(arguments, customLogger: (_, message) => Logger.Normal(message));
    }

    void SignWithSignTool(AbsolutePath[] files, string url)
    {
        Logger.Info("Signing files using signtool.");

        SignToolLogger = (_, message) => Logger.Normal(message);

        SignTool(_ => _
            .SetFile(SigningCertificatePath)
            .SetPassword(SigningCertificatePassword)
            .SetFiles(files.Select(x => x.ToString()).ToArray())
            .SetProcessToolPath(RootDirectory / "certificates" / "signtool.exe")
            .SetTimestampServerDigestAlgorithm("sha256")
            .SetDescription("Octopus Client Library")
            .SetUrl("https://octopus.com")
            .SetRfc3161TimestampServerUrl(url));
    }

    void ReplaceTextInFiles(AbsolutePath path, string oldValue, string newValue)
    {
        var fileText = File.ReadAllText(path);
        fileText = fileText.Replace(oldValue, newValue);
        File.WriteAllText(path, fileText);
    }
}
