using Octopus.Client.Model.Git;

namespace Octopus.Client.Model;

public class ReleaseTemplateGitResource
{
    public ReleaseTemplateGitResource(string actionName, string repositoryUri, string defaultBranch, string[] filePaths, bool isResolvable)
    {
        ActionName = actionName;
        RepositoryUri = repositoryUri;
        DefaultBranch = defaultBranch;
        FilePaths = filePaths;
        IsResolvable = isResolvable;
    }
    
    public string ActionName { get; }
    public string RepositoryUri { get; }
    public string DefaultBranch { get; }
    public bool IsResolvable { get; }
    public string[] FilePaths {get;}
    public string GitCredentialId { get; set; }
    public GitReferenceResource GitResourceSelectedLastRelease { get; set; }
}