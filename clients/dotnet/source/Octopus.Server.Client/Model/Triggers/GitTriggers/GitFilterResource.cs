#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model.Triggers.GitTriggers;

public class GitFilterResource : TriggerFilterResource
{
    public override TriggerFilterType FilterType => TriggerFilterType.GitFilter;
    
    public List<GitTriggerSourceResource> Sources { get; set; } = [];
}

public class GitTriggerSourceResource
{
    [JsonConstructor]
    public GitTriggerSourceResource(string deploymentActionSlug, string gitDependencyName, string[] includeFilePaths, string[] excludeFilePaths)
    {
        DeploymentActionSlug = deploymentActionSlug;
        GitDependencyName = gitDependencyName;
        IncludeFilePaths = includeFilePaths;
        ExcludeFilePaths = excludeFilePaths;
    }

    public string DeploymentActionSlug { get; set; }
    public string GitDependencyName { get; set; }
    public string[] IncludeFilePaths { get; set; }
    public string[] ExcludeFilePaths { get; set; }
}
