//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&prerelease"
#tool "nuget:?package=ILRepack"
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
var octoPublishFolder = Path.Combine(publishDir, "Octo");
var octoMergedFolder = Path.Combine(publishDir, "OctoMerged");

var gitVersionInfo = GitVersion(new GitVersionSettings {
    OutputType = GitVersionOutput.Json
});

var nugetVersion = gitVersionInfo.NuGetVersion;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    Information("Building OctopusClients v{0}", nugetVersion);
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
    .IsDependentOn("__Publish")
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
        BuildSystem.TeamCity.SetBuildNumber(gitVersionInfo.NuGetVersion);
    if(BuildSystem.IsRunningOnAppVeyor)
        AppVeyor.UpdateBuildVersion(gitVersionInfo.NuGetVersion);
});

Task("__Build")
    .IsDependentOn("__UpdateProjectJsonVersion")
    .IsDependentOn("__UpdateAssemblyVersionInformation")
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
        OutputDirectory = octoPublishFolder
    });
});

Task("__MergeOctoExe")
    .Does(() => {
        CreateDirectory(octoMergedFolder);
        ILRepack(
            Path.Combine(octoMergedFolder, "Octo.exe"),
            Path.Combine(octoPublishFolder, "Octo.exe"),
            IO.Directory.EnumerateFiles(octoPublishFolder, "*.dll").Select(f => (FilePath) f),
            new ILRepackSettings { 
                Internalize = true, 
                Libs = new List<FilePath>() { octoPublishFolder }
            }
        );
        DeleteFile(Path.Combine(octoMergedFolder, "Octo.pdb"));
        CopyFileToDirectory(Path.Combine(octoPublishFolder, "Octo.exe.config"), octoMergedFolder);
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
    .IsDependentOn("__MergeOctoExe")
    .Does(() => {
        var nugetPackDir = Path.Combine(publishDir, "nuget");
        var nuspecFile = "OctopusTools.nuspec";
        
        CopyDirectory(octoMergedFolder, nugetPackDir);
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
