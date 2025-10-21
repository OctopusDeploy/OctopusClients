#nullable enable
using Octopus.Client.Model.Git;

namespace Octopus.Client.Model;

public class ReleaseTemplateGitResource
{
    public ReleaseTemplateGitResource(string name, string actionName, string repositoryUri, string defaultBranch, string[] filePathFilters, bool isResolvable)
    {
        Name = name;
        ActionName = actionName;
        RepositoryUri = repositoryUri;
        DefaultBranch = defaultBranch;
        FilePathFilters = filePathFilters;
        IsResolvable = isResolvable;
    }

    public string Name { get; set; }
    public string ActionName { get; }
    public string RepositoryUri { get; }
    public string DefaultBranch { get; }
    public bool IsResolvable { get; }
    public string[] FilePathFilters { get; }
    public string? GitCredentialId { get; set; }
    public string? GitHubConnectionId { get; set; }
    public GitReferenceResource? GitResourceSelectedLastRelease { get; set; }
}