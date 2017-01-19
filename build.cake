//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0007"
#tool "nuget:?package=ILRepack&version=2.0.11"
#addin "nuget:?package=Newtonsoft.Json&version=9.0.1"
#addin "nuget:?package=SharpCompress&version=0.12.4"

using Path = System.IO.Path;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IO = System.IO;
using SharpCompress;
using SharpCompress.Common;
using SharpCompress.Writer;

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
var projectToPublish = "./source/Octo";
var projectToPublishProjectJson = Path.Combine(projectToPublish, "project.json");
var octopusClientFolder = "./source/Octopus.Client";
var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;
var octoPublishFolder = Path.Combine(publishDir, "Octo");
var octoMergedFolder = Path.Combine(publishDir, "OctoMerged");
var cleanups = new List<Action>(); 

var gitVersionInfo = GitVersion(new GitVersionSettings {
    OutputType = GitVersionOutput.Json
});

var nugetVersion = gitVersionInfo.NuGetVersion;
var runtimes = new[] {
    "win7-x86", 
    "osx.10.10-x64",
    "ubuntu.14.04-x64",
    "ubuntu.16.04-x64",
    "rhel.7-x64",
    "debian.8-x64",
    "fedora.23-x64",
    "opensuse.13.2-x64",
    "linuxmint.17-x64",
};

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    if(BuildSystem.IsRunningOnTeamCity)
        BuildSystem.TeamCity.SetBuildNumber(gitVersionInfo.NuGetVersion);
    if(BuildSystem.IsRunningOnAppVeyor)
        AppVeyor.UpdateBuildVersion(gitVersionInfo.NuGetVersion);

    Information("Building OctopusClients v{0}", nugetVersion);
});

Teardown(context =>
{
    Information("Cleaning up");
    foreach(var cleanup in cleanups)
        cleanup();

    Information("Finished running tasks for build v{0}", nugetVersion);
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("__Default")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Restore")
    .IsDependentOn("__Build")
    .IsDependentOn("__Test")
    .IsDependentOn("__UpdateProjectJsonVersion")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Zip")
    .IsDependentOn("__PackNuget")
    .IsDependentOn("__CopyToLocalPackages");

Task("__Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectory(publishDir);
    CleanDirectories("./source/**/bin");
    CleanDirectories("./source/**/obj");
});

Task("__Restore")
    .Does(() => DotNetCoreRestore());

Task("__Build")
    .IsDependentOn("__UpdateProjectJsonVersion")
    .Does(() =>
{
    DotNetCoreBuild("**/project.json", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("__Test")
    .WithCriteria(false)
    .Does(() =>
{
    GetFiles("**/*Tests/project.json")
        .ToList()
        .ForEach(testProjectFile => 
        {
            DotNetCoreTest(testProjectFile.ToString(), new DotNetCoreTestSettings
            {
                Configuration = configuration,
                WorkingDirectory = Path.GetDirectoryName(testProjectFile.ToString())
            });
        });
});

Task("__UpdateProjectJsonVersion")
    .Does(() =>
{
    foreach(var projectJson in GetFiles("**/project.json").Select(p => p.FullPath))
    {
        RestoreFileOnCleanup(projectJson);
        Information("Updating {0} version -> {1}", projectJson, nugetVersion);
        ModifyJson(projectJson, json => json["version"] = nugetVersion);
    }
});

Task("__Publish")
    .Does(() =>
{
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Framework = "net45",
        Configuration = configuration,
        OutputDirectory = $"{octoPublishFolder}/netfx"
    });

    var portablePublishDir = Path.Combine(octoPublishFolder, "portable");
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Framework = "netcoreapp1.0",
        Configuration = configuration,
        OutputDirectory = portablePublishDir
    });
    CopyFileToDirectory(Path.Combine(assetDir, "Octo"), portablePublishDir);
    CopyFileToDirectory(Path.Combine(assetDir, "Octo.cmd"), portablePublishDir);

    ConvertToJsonOutput(projectToPublishProjectJson);
    DotNetCoreRestore();

    foreach(var runtime in runtimes)
        DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
        {
            Framework = "netcoreapp1.0",
            Configuration = configuration,
            Runtime = runtime,
            OutputDirectory = Path.Combine(octoPublishFolder, runtime)
        });
});

private void ConvertToJsonOutput(string projectJson)
{
    ModifyJson(projectJson, json => {
        var deps = (JObject)json["frameworks"]["netcoreapp1.0"]["dependencies"];
        deps.Remove("Microsoft.NETCore.App");
        deps["Microsoft.NETCore.Runtime.CoreCLR"] = new JValue("1.0.4");
        deps["Microsoft.NETCore.DotNetHostPolicy"] = new JValue("1.0.1");
        json["runtimes"] = new JObject();
        foreach (var runtime in runtimes)
            json["runtimes"][runtime] = new JObject();
    });
}

private void ModifyJson(string jsonFile, Action<JObject> modify)
{
    var json = JsonConvert.DeserializeObject<JObject>(IO.File.ReadAllText(jsonFile));
    modify(json);
    IO.File.WriteAllText(jsonFile, JsonConvert.SerializeObject(json, Formatting.Indented));
}

private void RestoreFileOnCleanup(string file)
{
    var contents = System.IO.File.ReadAllBytes(file);
    cleanups.Add(() => {
        Information("Restoring {0}", file);
        System.IO.File.WriteAllBytes(file, contents);
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

        using (Stream stream = IO.File.Open(outFile, FileMode.Create))
        using (var zip = WriterFactory.Open(stream, ArchiveType.GZip, CompressionType.GZip))
            zip.Write($"{outputFile}.tar", tarMemStream);
    }
    Information("Successfully created TGZ file: {0}", outFile);
}

Task("__MergeOctoExe")
    .Does(() => {
        var inputFolder = $"{octoPublishFolder}/netfx";
        var outputFolder = $"{octoPublishFolder}/netfx-merged";
        CreateDirectory(outputFolder);
        ILRepack(
            $"{outputFolder}/Octo.exe",
            $"{inputFolder}/Octo.exe",
            IO.Directory.EnumerateFiles(inputFolder, "*.dll").Select(f => (FilePath) f),
            new ILRepackSettings { 
                Internalize = true, 
                Libs = new List<FilePath>() { inputFolder }
            }
        );
    });



Task("__Zip")
    .IsDependentOn("__MergeOctoExe")
    .IsDependentOn("__Publish")
    .Does(() => {


        foreach(var dir in IO.Directory.EnumerateDirectories(octoPublishFolder))
        {
            var dirName = Path.GetFileName(dir);

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

Task("__PackNuget")
    .IsDependentOn("__PackClientNuget")
    .IsDependentOn("__MergeOctoExe")
    .IsDependentOn("__Publish")
    .IsDependentOn("__PackOctopusToolsNuget");

Task("__PackClientNuget")
    .Does(() => {
        var inputFolder = $"{octopusClientFolder}/bin/{configuration}/net45";
        var outputFolder = $"{octopusClientFolder}/bin/{configuration}/net45Merged";
        CreateDirectory(outputFolder);
        ILRepack(
            $"{outputFolder}/Octopus.Client.dll",
            $"{inputFolder}/Octopus.Client.dll",
            IO.Directory.EnumerateFiles(inputFolder, "*.dll").Select(f => (FilePath) f),
            new ILRepackSettings { 
                Internalize = true, 
                Libs = new List<FilePath>() { inputFolder }
            }
        );
        DeleteDirectory(inputFolder, true);
        MoveDirectory(outputFolder, inputFolder);

        DotNetCorePack(octopusClientFolder, new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = artifactsDir,
            NoBuild = true
        });
    });

Task("__PackOctopusToolsNuget")
    .Does(() => {
        var nugetPackDir = Path.Combine(publishDir, "nuget");
        var nuspecFile = "OctopusTools.nuspec";
        
        CopyDirectory($"{octoPublishFolder}/netfx-merged", nugetPackDir);
        CopyFileToDirectory(Path.Combine(assetDir, "init.ps1"), nugetPackDir);
        CopyFileToDirectory(Path.Combine(assetDir, nuspecFile), nugetPackDir);

        NuGetPack(Path.Combine(nugetPackDir, nuspecFile), new NuGetPackSettings {
            Version = nugetVersion,
            OutputDirectory = artifactsDir
        });
    });

Task("__CopyToLocalPackages")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .IsDependentOn("__PackClientNuget")
    .Does(() =>
{
    CreateDirectory(localPackagesDir);
    CopyFileToDirectory(Path.Combine(artifactsDir, $"Octopus.Client.{nugetVersion}.nupkg"), localPackagesDir);
});


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("__Default");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);