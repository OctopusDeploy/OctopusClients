//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&prerelease"
#addin "nuget:?package=Newtonsoft.Json"
#addin "nuget:?package=SharpCompress"

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
var globalAssemblyFile = "./source/Octo/Properties/AssemblyInfo.cs";
var projectToPublish = "./source/Octo";
var projectToPublishProjectJson = Path.Combine(projectToPublish, "project.json");
var octopusClientFolder = "./source/Octopus.Client";
var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;

var gitVersionInfo = GitVersion(new GitVersionSettings {
    OutputType = GitVersionOutput.Json
});

var nugetVersion = gitVersionInfo.NuGetVersion;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    Information("Building Octo.exe v{0}", nugetVersion);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("__Default")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Restore")
    .IsDependentOn("__UpdateAssemblyVersionInformation")
    .IsDependentOn("__Build")
    .IsDependentOn("__Test")
    .IsDependentOn("__UpdateProjectJsonVersion")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Zip")
    .IsDependentOn("__PackNuget");

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

Task("__UpdateAssemblyVersionInformation")
    .WithCriteria(isContinuousIntegrationBuild)
    .Does(() =>
{
     GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true,
        UpdateAssemblyInfoFilePath = globalAssemblyFile
    });

    Information("AssemblyVersion -> {0}", gitVersionInfo.AssemblySemVer);
    Information("AssemblyFileVersion -> {0}", $"{gitVersionInfo.MajorMinorPatch}.0");
    Information("AssemblyInformationalVersion -> {0}", gitVersionInfo.InformationalVersion);
    if(BuildSystem.IsRunningOnTeamCity)
        BuildSystem.TeamCity.SetBuildNumber(gitVersionInfo.InformationalVersion);
});

Task("__Build")
    .Does(() =>
{
    DotNetCoreBuild("**/project.json", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("__Test")
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
    .WithCriteria(isContinuousIntegrationBuild)
    .Does(() =>
{
    Information("Updating {0} version -> {1}", projectToPublishProjectJson, nugetVersion);
    ModifyJson(Path.Combine(octopusClientFolder, "project.json"), json => json["version"] = nugetVersion);
    ModifyJson(projectToPublishProjectJson, json => json["version"] = nugetVersion);
});

private void ModifyJson(string jsonFile, Action<JObject> modify)
{
    var json = JsonConvert.DeserializeObject<JObject>(IO.File.ReadAllText(jsonFile));
    modify(json);
    IO.File.WriteAllText(jsonFile, JsonConvert.SerializeObject(json, Formatting.Indented));
}


Task("__Publish")
    .Does(() =>
{
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory =  Path.Combine(publishDir, "Octo.exe")
    });
});


Task("__Zip")
    .IsDependentOn("__Publish")
    .Does(() => {
        var dir = Path.Combine(publishDir, "Octo.exe");
        var outFile = Path.Combine(artifactsDir, $"Octo.exe.zip");
        Zip(dir, outFile);
    });

Task("__PackNuget")
    .IsDependentOn("__Publish")
    .IsDependentOn("__PackOctopusToolsNuget")
    .IsDependentOn("__PackClientNuget");

Task("__PackClientNuget")
    .Does(() => {
        DotNetCorePack(octopusClientFolder, new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = artifactsDir
        });
    });

Task("__PackOctopusToolsNuget")
    .Does(() => {
        var nugetPackDir = Path.Combine(publishDir, "nuget");
        var nuspecFile = "OctopusTools.nuspec";
        
        CopyDirectory(Path.Combine(publishDir, "Octo.exe"), nugetPackDir);
        CopyFileToDirectory(Path.Combine(assetDir, "init.ps1"), nugetPackDir);
        CopyFileToDirectory(Path.Combine(assetDir, nuspecFile), nugetPackDir);

        NuGetPack(Path.Combine(nugetPackDir, nuspecFile), new NuGetPackSettings {
            Version = nugetVersion,
            OutputDirectory = artifactsDir
        });
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
