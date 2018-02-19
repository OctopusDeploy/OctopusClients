//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0011"
#tool "nuget:?package=ILRepack&version=2.0.13"
#addin "nuget:?package=SharpCompress&version=0.12.4"

using SharpCompress;
using SharpCompress.Common;
using SharpCompress.Writer;
using System.Xml;

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
var assetDir = "./BuildAssets";
var localPackagesDir = "../LocalPackages";
var globalAssemblyFile = "./source/Octo/Properties/AssemblyInfo.cs";
var projectToPublish = "./source/Octo/Octo.csproj";
var octopusClientFolder = "./source/Octopus.Client";
var octoPublishFolder = $"{publishDir}/Octo";
var octoMergedFolder =  $"{publishDir}/OctoMerged";
var octopusCliFolder = "./source/Octopus.Cli";
var dotNetOctoCliFolder = "./source/Octopus.DotNet.Cli";
var dotNetOctoPublishFolder = $"{publishDir}/dotnetocto";
var dotNetOctoMergedFolder =  $"{publishDir}/dotnetocto-Merged";

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
        GetFiles("**/**/*Tests.csproj")
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

Task("DotnetPublish")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Framework = "net451",
        Configuration = configuration,
        OutputDirectory = $"{octoPublishFolder}/netfx",
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });

    var portablePublishDir =  $"{octoPublishFolder}/portable";
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Framework = "netcoreapp2.0",
        Configuration = configuration,
        OutputDirectory = portablePublishDir,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
    SignBinaries(portablePublishDir);
    
    CopyFileToDirectory($"{assetDir}/Octo", portablePublishDir);
    CopyFileToDirectory($"{assetDir}/Octo.cmd", portablePublishDir);

    var doc = new XmlDocument();
    doc.Load(@".\source\Octo\Octo.csproj");
    var rids = doc.SelectSingleNode("Project/PropertyGroup/RuntimeIdentifiers").InnerText;
    foreach (var rid in rids.Split(';'))
    {
        DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
        {
            Framework = "netcoreapp2.0",
            Configuration = configuration,
            Runtime = rid,
            OutputDirectory = $"{octoPublishFolder}/{rid}",
			ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
        });
        SignBinaries($"{octoPublishFolder}/{rid}");
    }
    
});


Task("MergeOctoExe")
    .IsDependentOn("DotnetPublish")
    .Does(() => {
        var inputFolder = $"{octoPublishFolder}/netfx";
        var outputFolder = $"{octoPublishFolder}/netfx-merged";
        CreateDirectory(outputFolder);
        ILRepack(
            $"{outputFolder}/Octo.exe",
            $"{inputFolder}/Octo.exe",
            System.IO.Directory.EnumerateFiles(inputFolder, "*.dll").Select(f => (FilePath) f),
            new ILRepackSettings {
                Internalize = true,
                Parallel = true,
                Libs = new List<DirectoryPath>() { inputFolder }
            }
        );
        SignBinaries(outputFolder);
    });

	
Task("Zip")
    .IsDependentOn("MergeOctoExe")
    .IsDependentOn("DotnetPublish")
    .Does(() => {


        foreach(var dir in System.IO.Directory.EnumerateDirectories(octoPublishFolder))
        {
            var dirName = System.IO.Path.GetFileName(dir);

            if(dirName == "netfx")
                continue;

            if(dirName == "netfx-merged")
            {
                Zip(dir, $"{artifactsDir}/OctopusTools.{nugetVersion}.zip");
            }
            else
            {
                var outFile = $"{artifactsDir}/OctopusTools.{nugetVersion}.{dirName}";
                if(dirName == "portable" || dirName.Contains("win"))
                    Zip(dir, outFile + ".zip");

                if(!dirName.Contains("win"))
                    TarGzip(dir, outFile);
            }
        }
    });


Task("PackClientNuget")
    .IsDependentOn("Test")
    .Does(() => {
        var inputFolder = $"{octopusClientFolder}/bin/{configuration}/net45";
        var outputFolder = $"{octopusClientFolder}/bin/{configuration}/net45Merged";
        CreateDirectory(outputFolder);
        ILRepack(
            $"{outputFolder}/Octopus.Client.dll",
            $"{inputFolder}/Octopus.Client.dll",
            System.IO.Directory.EnumerateFiles(inputFolder, "NewtonSoft.Json.dll").Select(f => (FilePath) f),
            new ILRepackSettings {
                Internalize = true,
                Parallel = false,
                XmlDocs = true,
                Libs = new List<DirectoryPath>() { inputFolder }
            }
        );
        DeleteDirectory(inputFolder, true);
        MoveDirectory(outputFolder, inputFolder);

        SignBinaries($"{octopusClientFolder}/bin/{configuration}");

        DotNetCorePack(octopusClientFolder, new DotNetCorePackSettings {
            ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}"),
            Configuration = configuration,
            OutputDirectory = artifactsDir,
            NoBuild = true,
            IncludeSymbols = false
        });
    });


Task("PackOctopusToolsNuget")
    .IsDependentOn("MergeOctoExe")
    .Does(() => {
        var nugetPackDir = $"{publishDir}/nuget";
        var nuspecFile = "OctopusTools.nuspec";

        CopyDirectory($"{octoPublishFolder}/netfx-merged", nugetPackDir);
        CopyFileToDirectory($"{assetDir}/LICENSE.txt", nugetPackDir);
        CopyFileToDirectory($"{assetDir}/VERIFICATION.txt", nugetPackDir);
        CopyFileToDirectory($"{assetDir}/init.ps1", nugetPackDir);
        CopyFileToDirectory($"{assetDir}/{nuspecFile}", nugetPackDir);

        NuGetPack($"{nugetPackDir}/{nuspecFile}", new NuGetPackSettings {
            Version = nugetVersion,
            OutputDirectory = artifactsDir
        });
    });

Task("PackDotNetOctoNuget")
	.IsDependentOn("DotnetPublish")
    .Does(() => {

		SignBinaries($"{octopusCliFolder}/bin/{configuration}");

		DotNetCorePack(octopusCliFolder, new DotNetCorePackSettings
		{
			Configuration = configuration,
			OutputDirectory = artifactsDir,
			ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}"),
            NoBuild = true,
            IncludeSymbols = false
		});

		SignBinaries($"{dotNetOctoCliFolder}/bin/{configuration}");

		DotNetCorePack(dotNetOctoCliFolder, new DotNetCorePackSettings
		{
			Configuration = configuration,
			OutputDirectory = artifactsDir,
			ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}"),
            NoBuild = true,
            IncludeSymbols = false
		});
    });

Task("CopyToLocalPackages")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .IsDependentOn("PackClientNuget")
    .IsDependentOn("PackOctopusToolsNuget")
    .IsDependentOn("PackDotNetOctoNuget")
    .IsDependentOn("Zip")
    .Does(() =>
{
    CreateDirectory(localPackagesDir);
    CopyFileToDirectory($"{artifactsDir}/Octopus.Client.{nugetVersion}.nupkg", localPackagesDir);
    CopyFileToDirectory($"{artifactsDir}/Octopus.Cli.{nugetVersion}.nupkg", localPackagesDir);
    CopyFileToDirectory($"{artifactsDir}/Octopus.DotNet.Cli.{nugetVersion}.nupkg", localPackagesDir);
});

private void SignBinaries(string path)
{
    Information($"Signing binaries in {path}");
	var files = GetFiles(path + "/**/Octopus.*.dll");
    files.Add(GetFiles(path + "/**/Octo.dll"));
    files.Add(GetFiles(path + "/**/Octo.exe"));
    files.Add(GetFiles(path + "/**/dotnet-octo.dll"));

	Sign(files, new SignToolSignSettings {
			ToolPath = MakeAbsolute(File("./certificates/signtool.exe")),
            TimeStampUri = new Uri("http://timestamp.globalsign.com/scripts/timestamp.dll"),
            CertPath = signingCertificatePath,
            Password = signingCertificatePassword
    });
}


private void TarGzip(string path, string outputFile)
{
    var outFile = $"{outputFile}.tar.gz";
    Information("Creating TGZ file {0} from {1}", outFile, path);
    using (var tarMemStream = new MemoryStream())
    {
        using (var tar = WriterFactory.Open(tarMemStream, ArchiveType.Tar, CompressionType.None, true))
        {
            tar.WriteAll(path, "*", SearchOption.AllDirectories);
        }

        tarMemStream.Seek(0, SeekOrigin.Begin);

        using (Stream stream = System.IO.File.Open(outFile, FileMode.Create))
        using (var zip = WriterFactory.Open(stream, ArchiveType.GZip, CompressionType.GZip))
            zip.Write($"{outputFile}.tar", tarMemStream);
    }
    Information("Successfully created TGZ file: {0}", outFile);
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