//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=ILRepack&version=2.0.13"
#addin "nuget:?package=Cake.Incubator&version=4.0.0"
#addin "nuget:?package=Cake.FileHelpers&version=3.2.0"

using Cake.Incubator;
using Cake.Incubator.LoggingExtensions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var signingCertificatePath = Argument("signing_certificate_path", "");
var signingCertificatePassword = Argument("signing_certificate_password", "");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var publishDir = "./publish";
var artifactsDir = "./artifacts";
var localPackagesDir = "../LocalPackages";
var octopusClientFolder = "./source/Octopus.Client";

GitVersion gitVersionInfo;
string nugetVersion;


///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
     gitVersionInfo = GitVersion(new GitVersionSettings {
        OutputType = GitVersionOutput.Json
    });
    nugetVersion = gitVersionInfo.NuGetVersion;

    if(BuildSystem.IsRunningOnTeamCity)
        BuildSystem.TeamCity.SetBuildNumber(nugetVersion);

    Information("Building OctopusClients v{0}", nugetVersion);
    Information("Informational Version {0}", gitVersionInfo.InformationalVersion);
    Verbose("GitVersion:\n{0}", gitVersionInfo.Dump());
});

Teardown(context =>
{
    Information("Finished running tasks for build v{0}", nugetVersion);
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectory(publishDir);
    CleanDirectories("./source/**/bin");
    CleanDirectories("./source/**/obj");
    CleanDirectories("./source/**/TestResults");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => DotNetCoreRestore("source", new DotNetCoreRestoreSettings
        {
            ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
        }));

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreBuild("./source", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        GetFiles("**/**/*.Tests.csproj")
            .ToList()
            .ForEach(testProjectFile =>
            {
                DotNetCoreTest(testProjectFile.FullPath, new DotNetCoreTestSettings
                {
                    Configuration = configuration,
                    NoBuild = true
                });
            });
    });

Task("Merge")
    .IsDependentOn("Test")
    .Does(() => {
        var inputFolder = $"{octopusClientFolder}/bin/{configuration}/net452";
        var outputFolder = $"{octopusClientFolder}/bin/{configuration}/net452Merged";
        CreateDirectory(outputFolder);

        var assemblyPaths = System.IO.Directory.EnumerateFiles(inputFolder, "NewtonSoft.Json.dll").Select(f => (FilePath) f);
        assemblyPaths = assemblyPaths.Concat(System.IO.Directory.EnumerateFiles(inputFolder, "Octodiff.exe").Select(f => (FilePath) f));

        ILRepack(
            $"{outputFolder}/Octopus.Client.dll",
            $"{inputFolder}/Octopus.Client.dll",
            assemblyPaths,
            new ILRepackSettings {
                Internalize = true,
                Parallel = false,
                XmlDocs = true,
                Libs = new List<DirectoryPath>() { inputFolder }
            }
        );

        DeleteDirectory(inputFolder, true);
        MoveDirectory(outputFolder, inputFolder);
    });

Task("PackClientNuget")
    .IsDependentOn("Merge")
    .Does(() => {
        SignBinaries($"{octopusClientFolder}/bin/{configuration}");

        try
        {
            ReplaceTextInFiles($"{octopusClientFolder}/Octopus.Client.nuspec",
                "<version>$version$</version>",
                $"<version>{nugetVersion}</version>");

            DotNetCorePack(octopusClientFolder, new DotNetCorePackSettings {
                ArgumentCustomization = args => {
                    args.Append($"/p:Version={nugetVersion}");
                    args.Append($"/p:NuspecFile=Octopus.Client.nuspec");
                    return args;
                },
                Configuration = configuration,
                OutputDirectory = artifactsDir,
                NoBuild = true,
                IncludeSymbols = false,
                Verbosity = DotNetCoreVerbosity.Normal,
            });
        }
        finally
        {
            ReplaceTextInFiles($"{octopusClientFolder}/Octopus.Client.nuspec",
                            $"<version>{nugetVersion}</version>",
                            $"<version>$version$</version>");        
        }
    });
    
Task("TestClientNugetPackage")
    .IsDependentOn("PackClientNuget")
    .Does(() => {
        // tests that make sure the packed, ilmerged dll we're going to ship actually works the way we expect it to
        DotNetCoreTest("./source/Octopus.Client.E2ETests/Octopus.Client.E2ETests.csproj", new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = true
        });
    });


Task("CopyToLocalPackages")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .IsDependentOn("TestClientNugetPackage")
    .Does(() =>
{
    CreateDirectory(localPackagesDir);
    CopyFileToDirectory($"{artifactsDir}/Octopus.Client.{nugetVersion}.nupkg", localPackagesDir);
});

private void SignBinaries(string path)
{
    Information($"Signing binaries in {path}");
	var files = GetFiles(path + "/**/Octopus.*.dll");

	Sign(files, new SignToolSignSettings {
			ToolPath = MakeAbsolute(File("./certificates/signtool.exe")),
            TimeStampUri = new Uri("http://rfc3161timestamp.globalsign.com/advanced"),
            TimeStampDigestAlgorithm = SignToolDigestAlgorithm.Sha256,
            CertPath = signingCertificatePath,
            Password = signingCertificatePassword
    });
}


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("CopyToLocalPackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
