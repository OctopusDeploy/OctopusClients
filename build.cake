//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0011"
#tool "nuget:?package=ILRepack&version=2.0.11"
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
    .Does(() => DotNetCoreRestore("source"));

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
                    NoBuild = true,
                    ArgumentCustomization = args => args.Append("-l trx")
                });
            });
    });

Task("DotnetPublish")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Framework = "net45",
        Configuration = configuration,
        OutputDirectory = $"{octoPublishFolder}/netfx"
    });

    var portablePublishDir =  $"{octoPublishFolder}/portable";
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Framework = "netcoreapp1.0",
        Configuration = configuration,
        OutputDirectory = portablePublishDir
    });
    CopyFileToDirectory($"{assetDir}/Octo", portablePublishDir);
    CopyFileToDirectory($"{assetDir}/Octo.cmd", portablePublishDir);

    var doc = new XmlDocument();
    doc.Load(@".\source\Octo\Octo.csproj");
    var rids = doc.SelectSingleNode("Project/PropertyGroup/RuntimeIdentifiers").InnerText;
    foreach (var rid in rids.Split(';'))
    {
        DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
        {
            Framework = "netcoreapp1.0",
            Configuration = configuration,
            Runtime = rid,
            OutputDirectory = $"{octoPublishFolder}/{rid}"
        });
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
                Libs = new List<FilePath>() { inputFolder }
            }
        );
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
            System.IO.Directory.EnumerateFiles(inputFolder, "*.dll").Select(f => (FilePath) f),
            new ILRepackSettings { 
                Internalize = true,
                XmlDocs = true, 
                Libs = new List<FilePath>() { inputFolder }
            }
        );
        DeleteDirectory(inputFolder, true);
        MoveDirectory(outputFolder, inputFolder);

        DotNetCorePack(octopusClientFolder, new DotNetCorePackSettings {
            ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}"),
            Configuration = configuration,
            OutputDirectory = artifactsDir,
            NoBuild = true
        });
    });

    

Task("PackOctopusToolsNuget")
    .IsDependentOn("MergeOctoExe")
    .Does(() => {
        var nugetPackDir = $"{publishDir}/nuget";
        var nuspecFile = "OctopusTools.nuspec";
        
        CopyDirectory($"{octoPublishFolder}/netfx-merged", nugetPackDir);
        CopyFileToDirectory($"{assetDir}/init.ps1", nugetPackDir);
        CopyFileToDirectory($"{assetDir}/{nuspecFile}", nugetPackDir);

        NuGetPack($"{nugetPackDir}/{nuspecFile}", new NuGetPackSettings {
            Version = nugetVersion,
            OutputDirectory = artifactsDir
        });
    });

Task("CopyToLocalPackages")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .IsDependentOn("PackClientNuget")
    .IsDependentOn("PackOctopusToolsNuget")
    .IsDependentOn("Zip")
    .Does(() =>
{
    CreateDirectory(localPackagesDir);
    CopyFileToDirectory($"{artifactsDir}/Octopus.Client.{nugetVersion}.nupkg", localPackagesDir);
});


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