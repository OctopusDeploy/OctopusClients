//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine"
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
var globalAssemblyFile = "./source/Octo/Properties/AssemblyInfo.cs";
var projectToPublish = "./source/Octo";
var projectToPublishProjectJson = Path.Combine(projectToPublish,"project.json");

var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;

var gitVersionInfo = GitVersion(new GitVersionSettings {
    OutputType = GitVersionOutput.Json
});

var nugetVersion = gitVersionInfo.NuGetVersion;
var runtimes = new[] { 
    "win7-x64"
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
    .IsDependentOn("__Zip")
    .IsDependentOn("__Push");

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
    ModifyJson(projectToPublishProjectJson, json => json["version"] = nugetVersion);
});

Task("__Publish")
    .Does(() =>
{
    DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = Path.Combine(publishDir, "portable")
    });
   

    using(new AutoRestoreFile(projectToPublishProjectJson))
    {
        ConvertToJsonOutput(projectToPublishProjectJson);
        DotNetCoreRestore();

        foreach(var runtime in runtimes)
            DotNetCorePublish(projectToPublish, new DotNetCorePublishSettings
            {
                Configuration = configuration,
                Runtime = runtime,
                OutputDirectory = Path.Combine(publishDir, runtime)
            });
    } 
});

private void ConvertToJsonOutput(string projectJson)
{
    ModifyJson(projectJson, json => {
        var deps = (JObject)json["dependencies"];
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

private class AutoRestoreFile : IDisposable
{
	private byte[] _contents;
	private string _filename;
	public AutoRestoreFile(string filename)
	{
		_filename = filename;
		_contents = IO.File.ReadAllBytes(filename);
	}

	public void Dispose() => IO.File.WriteAllBytes(_filename, _contents);
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

Task("__Zip")
    .Does(() => {
        foreach(var dir in IO.Directory.EnumerateDirectories(publishDir))
        {
            var dirName = Path.GetFileName(dir);
            var outFile = Path.Combine(artifactsDir, $"Octo.exe.{dirName}");
            if(dirName.StartsWith("win") || dirName == "portable")
                Zip(dir, outFile + ".zip");

            if(!dirName.StartsWith("win"))
                TarGzip(dir, outFile);
        }
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
