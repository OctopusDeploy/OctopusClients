//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine"
#addin "MagicChunks"

using Path = System.IO.Path;

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
var globalAssemblyFile = "./source/Octo/Properties/AssemblyInfo.cs";
var projectToPublish = "./source/Octo";
var projectToPublishProjectJson = Path.Combine(projectToPublish,"project.json");

var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;

var gitVersionInfo = GitVersion(new GitVersionSettings {
    OutputType = GitVersionOutput.Json
});

var nugetVersion = gitVersionInfo.NuGetVersion;
var runtimes = new[] { 
    "win7-x64", 
    "ubuntu.14.04-x64",  
    "ubuntu.16.04-x64",
    "centos.7-x64",
    "osx.10.10-x64",
    "osx.10.11-x64"
};

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
    .IsDependentOn("__Push");

Task("__Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
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

    TransformConfig(projectToPublishProjectJson, projectToPublishProjectJson, new TransformationCollection {
        { "version", nugetVersion }
    });
});

Task("__Publish")
    .Does(() =>
{
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = Path.Combine(publishDir, "portable")
    });

    var backup = projectToPublishProjectJson + ".bak";
    CopyFile(projectToPublishProjectJson, backup);

    // TODO: change this if https://github.com/sergeyzwezdin/magic-chunks/pull/10 gets merged
    System.IO.File.WriteAllText(
        projectToPublishProjectJson,
        System.IO.File.ReadAllText(projectToPublishProjectJson)
            .Replace("Microsoft.NETCore.App", "Microsoft.NETCore.Runtime.CoreCLR")
    );
    var transform = new TransformationCollection {
        { "dependencies/Microsoft.NETCore.Runtime.CoreCLR", "1.0.4"},
        { "dependencies/Microsoft.NETCore.DotNetHostPolicy", "1.0.1"},
    };
    foreach(var runtime in runtimes)
        transform[$"runtimes/{runtime}"] = "";

    TransformConfig(projectToPublishProjectJson, projectToPublishProjectJson, transform);

    DotNetCoreRestore();

    foreach(var runtime in runtimes)
        DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
        {
            Configuration = configuration,
            Runtime = runtime,
            OutputDirectory = Path.Combine(publishDir, runtime)
        });
    DeleteFile(projectToPublishProjectJson);
    MoveFile(backup, projectToPublishProjectJson);        
});

Task("__Push")
    .WithCriteria(isContinuousIntegrationBuild)
    .Does(() =>
{
    var isPullRequest = !String.IsNullOrEmpty(EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"));
    var isMasterBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH") == "master" && !isPullRequest;
    var shouldPushToMyGet = !BuildSystem.IsLocalBuild;
    var shouldPushToNuGet = !BuildSystem.IsLocalBuild && isMasterBranch;

    if (shouldPushToMyGet)
    {
        NuGetPush("artifacts/Octostache." + nugetVersion + ".nupkg", new NuGetPushSettings {
            Source = "https://octopus.myget.org/F/octopus-dependencies/api/v3/index.json",
            ApiKey = EnvironmentVariable("MyGetApiKey")
        });
        NuGetPush("artifacts/Octostache." + nugetVersion + ".symbols.nupkg", new NuGetPushSettings {
            Source = "https://octopus.myget.org/F/octopus-dependencies/api/v3/index.json",
            ApiKey = EnvironmentVariable("MyGetApiKey")
        });
    }
    if (shouldPushToNuGet)
    {
        NuGetPush("artifacts/Octostache." + nugetVersion + ".nupkg", new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = EnvironmentVariable("NuGetApiKey")
        });
        NuGetPush("artifacts/Octostache." + nugetVersion + ".symbols.nupkg", new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = EnvironmentVariable("NuGetApiKey")
        });
    }
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
