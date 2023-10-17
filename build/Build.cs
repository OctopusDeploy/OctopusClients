using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI.TeamCity;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.AzureSignTool;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.ILRepack;
using Nuke.Common.Tools.OctoVersion;
using Nuke.Common.Tools.SignTool;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Tools.DockerCompose;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SignTool.SignToolTasks;

[VerbosityMapping(typeof(DotNetVerbosity),
    Verbose = nameof(DotNetVerbosity.Diagnostic))]
class Build : NukeBuild
{
    const string CiBranchNameEnvVariable = "OCTOVERSION_CurrentBranch";

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
    [Parameter] string AzureKeyVaultTenantId = "";

    ///////////////////////////////////////////////////////////////////////////////
    // GLOBAL VARIABLES
    ///////////////////////////////////////////////////////////////////////////////
    AbsolutePath PublishDir => RootDirectory / "publish";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath LocalPackagesDir => RootDirectory / ".." / "LocalPackages";
    AbsolutePath SourceDir => RootDirectory / "source";
    AbsolutePath OctopusClientFolder => SourceDir / "Octopus.Client";
    AbsolutePath OctopusNormalClientFolder => SourceDir / "Octopus.Server.Client";

    [Parameter("Whether to auto-detect the branch name - this is okay for a local build, but should not be used under CI.")]
    readonly bool AutoDetectBranch = IsLocalBuild;

    [Parameter("Branch name for OctoVersion to use to calculate the version number. Can be set via the environment variable " + CiBranchNameEnvVariable + ".", Name = CiBranchNameEnvVariable)]
    string BranchName { get; set; }

    [OctoVersion(Framework = "net6.0", BranchParameter = nameof(BranchName), AutoDetectBranchParameter = nameof(AutoDetectBranch))]
    public OctoVersionInfo OctoVersionInfo;

    static readonly string Timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

    string FullSemVer =>
        !IsLocalBuild
            ? OctoVersionInfo.FullSemVer
            : $"{OctoVersionInfo.FullSemVer}-{Timestamp}";

    string NuGetVersion =>
        !IsLocalBuild
            ? OctoVersionInfo.NuGetVersion
            : $"{OctoVersionInfo.NuGetVersion}-{Timestamp}";

    // Keep this list in order by most likely to succeed
    string[] SigningTimestampUrls => new[]
    {
        "http://timestamp.digicert.com?alg=sha256",
        "http://timestamp.comodoca.com",
        "http://tsa.starfieldtech.com",
        "http://www.startssl.com/timestamp",
        "http://timestamp.comodoca.com/rfc3161",
        "http://timestamp.verisign.com/scripts/timstamp.dll",
    };

    Target Clean => _ => _
        .Executes(() =>
    {
        EnsureCleanDirectory(ArtifactsDir);
        EnsureCleanDirectory(PublishDir);
        SourceDir.GlobDirectories("**/bin").ForEach(EnsureCleanDirectory);
        SourceDir.GlobDirectories("**/obj").ForEach(EnsureCleanDirectory);
        SourceDir.GlobDirectories("**/TestResults").ForEach(EnsureCleanDirectory);
        DeleteFile(LocalPackagesDir / $"Octopus.Client.{FullSemVer}.nupkg");
        DeleteFile(LocalPackagesDir / $"Octopus.Server.Client.{FullSemVer}.nupkg");
    });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(SourceDir)
                .SetVersion(FullSemVer));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
    {
        DotNetBuild(_ => _
            .SetProjectFile(SourceDir)
            .SetConfiguration(Configuration)
            .SetVersion(FullSemVer));
    });

    Target Merge => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        foreach (var target in new[] { "net462", "net48", "netstandard2.0" })
        {
            var inputFolder = OctopusClientFolder / "bin" / Configuration / target;
            var outputFolder = OctopusClientFolder / "bin" / Configuration / $"{target}Merged";
            EnsureExistingDirectory(outputFolder);

            // CAREFUL: We don't want to expose third-party libraries like Newtonsoft.Json so we definitely want to
            // internalize those, but we also don't want to hide any Octopus contracts.
            //
            // WARNING: There's an apparent bug in il-repack which ignores all types from subsequent assemblies, even
            // if a set of exclusion regular expressions is provided. To work around this, we do a two-stage merge:
            // 1) all of the Octopus assemblies into a temporary assembly, with internalization disabled entirely (leaving everything Octopus.* public); and
            // 2) that temporary assembly plus all of the third-party assemblies, leaving only the types from the first (Octopus temporary) assembly as public.
            // --andrewh 14/2/2022.

            // Stage 1: Merge all the Octopus assemblies whose contracts we want to not internalize.
            var stage1Assemblies = inputFolder.GlobFiles(
                    "Octopus.Server.Client.dll",
                    "Octopus.Server.MessageContracts.Base.dll",
                    "Octopus.Server.MessageContracts.Base.HttpRoutes.dll"
                )
                .Select(x => x.ToString())
                .OrderBy(x => x)
                .ToArray();

            var temporaryDllPath = inputFolder / "Octopus.Client.ILMerge.Temporary.dll";

            ILRepackTasks.ILRepack(_ => _
                .SetAssemblies(stage1Assemblies)
                .SetOutput(temporaryDllPath)
                .DisableParallel()
                .EnableXmldocs()
                .SetLib(inputFolder));

            // Step 2: Merge all the remaining assemblies whose innards will be marked as internal if they're currently public.
            var stage2Assemblies = inputFolder.GlobFiles("*.dll", "*.exe")
                .Select(x => x.ToString())
                .Except(stage1Assemblies)
                .OrderByDescending(x => x.Contains("Octopus.Client.ILMerge.Temporary.dll"))
                .ThenBy(x => x)
                .ToArray();

            var outputDllPath = outputFolder / "Octopus.Client.dll";

            ILRepackTasks.ILRepack(_ => _
                .SetAssemblies(stage2Assemblies)
                .SetOutput(outputDllPath)
                .EnableInternalize()
                .DisableParallel()
                .EnableXmldocs()
                .SetLib(inputFolder));

            DeleteDirectory(inputFolder);
            MoveDirectory(outputFolder, inputFolder);
        }
    });

    Target Test => _ => _
        .DependsOn(Compile)
        .DependsOn(Merge)   // IMPORTANT: Tests must be run _after_ the merge so that we're confident that we're testing the ILMerged code.  -andrewh 14/2/2022.
        .Executes(() =>
    {
        RootDirectory.GlobFiles("**/**/*.Tests.csproj").ForEach(testProjectFile =>
        {
            DotNetTest(_ => _
                .SetProjectFile(testProjectFile)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetLoggers("trx;LogFilePrefix=Win")
                .SetResultsDirectory("./TestResults/"));
        });
    });

    Target PackUnsignedNonMergedClientNuget => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .DependsOn(Compile)
        .Executes(() =>
    {
        Log.Warning("Building an Unsigned and non-packed Merged Client Nuget Package for MacOS");

        var octopusClientMacosNuspec = OctopusClientFolder / "Octopus.Client.Unsigned.Unpacked.nuspec";
        var projectFile = OctopusClientFolder / "Octopus.Client.csproj";

        ReplaceTextInFiles(octopusClientMacosNuspec, "<version>$version$</version>", $"<version>{FullSemVer}</version>");
        ReplaceTextInFiles(projectFile, "Octopus.Client.nuspec", "Octopus.Client.MacOs.nuspec");

        DotNetPack(_ => _
            .SetProject(OctopusClientFolder)
            .SetConfiguration(Configuration)
            .SetOutputDirectory(ArtifactsDir)
            .EnableNoBuild()
            .DisableIncludeSymbols()
            .SetVerbosity(DotNetVerbosity.Normal));

        // Put these back after so that future builds work and there are no pending changes locally.
        ReplaceTextInFiles(octopusClientMacosNuspec, $"<version>{FullSemVer}</version>", "<version>$version$</version>");
        ReplaceTextInFiles(projectFile, "Octopus.Client.Unsigned.Unpacked.nuspec", "Octopus.Client.nuspec");
    });

    Target PackSignedMergedClientNuget => _ => _
        .DependsOn(Merge)
        .Executes(() =>
    {
        SignBinaries(OctopusClientFolder / "bin" / Configuration);
        var octopusClientNuspec = OctopusClientFolder / "Octopus.Client.nuspec";
        try
        {
            ReplaceTextInFiles(octopusClientNuspec, "<version>$version$</version>",
                $"<version>{FullSemVer}</version>");

            DotNetPack(_ => _
                .SetProject(OctopusClientFolder)
                .SetProcessArgumentConfigurator(args =>
                {
                    args.Add($"/p:NuspecFile=Octopus.Client.nuspec");
                    return args;
                })
                .SetVersion(FullSemVer)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDir)
                .EnableNoBuild()
                .DisableIncludeSymbols()
                .SetVerbosity(DotNetVerbosity.Normal));
        }
        finally
        {
            ReplaceTextInFiles(octopusClientNuspec, $"<version>{FullSemVer}</version>", $"<version>$version$</version>");
        }
    });

    Target PackUnsignedNormalClientNugetForMacOs => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .DependsOn(Compile)
        .Executes(() =>
    {
        Log.Warning("Building an Unsigned Normal Client Nuget Package for MacOS");

        PackNormalClientNugetPackage();
    });

    Target PackSignedNormalClientNuget => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        SignBinaries(OctopusNormalClientFolder / "bin" / Configuration);

        PackNormalClientNugetPackage();
    });

    Target LocalTest => _ => _
        .DependsOn(Compile)
        .Executes(() =>
    {
        EnsureCleanDirectory(RootDirectory / "TestResults");

        DockerComposeBuild(RootDirectory / "docker-compose.build.yml", "--no-cache");
        DockerComposeUp(RootDirectory / "docker-compose.test.yml");
        DockerComposeDown(RootDirectory / "docker-compose.test.yml");

        var unitTestResultFiles = Directory.GetFiles(RootDirectory / "TestResults", "*.trx");

        Assert.Count(unitTestResultFiles, 11, "Incorrect number of results files found");
    });

    Target TestClientNugetPackage => _ => _
        .DependsOn(PackSignedMergedClientNuget)
        .Executes(() =>
    {
        // Tests that make sure the packed, ILMerged DLL we're going to ship actually works the way we expect it to.
        DotNetTest(_ => _
            .SetProjectFile(SourceDir / "Octopus.Client.E2ETests" / "Octopus.Client.E2ETests.csproj")
            .SetConfiguration(Configuration)
            .EnableNoBuild()
            .SetLoggers("trx;LogFilePrefix=Win-E2E")
            .SetResultsDirectory("./TestResults/"));
    });

    [PublicAPI]
    Target CopyToLocalPackages => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .DependsOn(PackSignedNormalClientNuget)
        .DependsOn(PackSignedMergedClientNuget)
        .Executes(() =>
    {
        EnsureExistingDirectory(LocalPackagesDir);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Client.{FullSemVer}.nupkg", LocalPackagesDir, FileExistsPolicy.Overwrite);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Server.Client.{FullSemVer}.nupkg", LocalPackagesDir, FileExistsPolicy.Overwrite);
    });

    [PublicAPI]
    Target CopyUnsignedNugetToLocalPackages => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .DependsOn(PackUnsignedNormalClientNugetForMacOs)
        .DependsOn(PackUnsignedNonMergedClientNuget)
        .Executes(() =>
    {
        Log.Warning("This build will produce an unsigned, non-packed nuget package - this is not suitable as a release candidate");

        EnsureExistingDirectory(LocalPackagesDir);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Client.{FullSemVer}.nupkg", LocalPackagesDir, FileExistsPolicy.Overwrite);
        CopyFileToDirectory($"{ArtifactsDir}/Octopus.Server.Client.{FullSemVer}.nupkg", LocalPackagesDir, FileExistsPolicy.Overwrite);
    });

    Target Default => _ => _
        .DependsOn(CopyToLocalPackages)
        .DependsOn(PackSignedNormalClientNuget)
        .DependsOn(PackSignedMergedClientNuget)
        .DependsOn(Test)
        .DependsOn(TestClientNugetPackage);

    void PackNormalClientNugetPackage()
    {
        DotNetPack(_ => _
            .SetProject(OctopusNormalClientFolder)
            .SetVersion(FullSemVer)
            .SetConfiguration(Configuration)
            .SetOutputDirectory(ArtifactsDir)
            .EnableNoBuild()
            .DisableIncludeSymbols()
            .SetVerbosity(DotNetVerbosity.Normal));
    }

    void SignBinaries(AbsolutePath path)
    {
        Log.Information($"Signing binaries in {path}");
        var files = path.GlobDirectories("**").SelectMany(x => x.GlobFiles("Octopus.*.dll")).ToArray();

        var useSignTool = string.IsNullOrEmpty(AzureKeyVaultUrl)
                          && string.IsNullOrEmpty(AzureKeyVaultAppId)
                          && string.IsNullOrEmpty(AzureKeyVaultAppSecret)
                          && string.IsNullOrEmpty(AzureKeyVaultCertificateName)
                          && string.IsNullOrEmpty(AzureKeyVaultTenantId);
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

        Log.Information($"Finished signing {files.Length} files.");
    }

    void SignWithAzureSignTool(AbsolutePath[] files, string timestampUrl)
    {
        Log.Information("Signing files using azuresigntool and the production code signing certificate.");

        AzureSignToolTasks.AzureSignTool(settings => settings
            .SetKeyVaultUrl(AzureKeyVaultUrl)
            .SetKeyVaultClientId(AzureKeyVaultAppId)
            .SetKeyVaultClientSecret(AzureKeyVaultAppSecret)
            .SetKeyVaultCertificateName(AzureKeyVaultCertificateName)
            .SetKeyVaultTenantId(AzureKeyVaultTenantId)
            .SetDescription("Octopus Client Library")
            .SetDescriptionUrl("https://octopus.com")
            .SetFileDigest(AzureSignToolDigestAlgorithm.sha256)
            .SetTimestampRfc3161Url(timestampUrl)
            .SetTimestampDigest(AzureSignToolDigestAlgorithm.sha256)
            .SetFiles(files.Select(x => x.ToString())));
    }

    void SignWithSignTool(AbsolutePath[] files, string url)
    {
        Log.Information("Signing files using signtool.");

        SignToolLogger = LogStdErrAsWarning;

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

    static void LogStdErrAsWarning(OutputType type, string message)
    {
        if (type == OutputType.Err)
            Log.Warning(message);
        else
            Log.Debug(message);
    }

    void ReplaceTextInFiles(AbsolutePath path, string oldValue, string newValue)
    {
        var fileText = File.ReadAllText(path);
        fileText = fileText.Replace(oldValue, newValue);
        File.WriteAllText(path, fileText);
    }
}