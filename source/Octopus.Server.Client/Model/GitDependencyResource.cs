#nullable enable

namespace Octopus.Client.Model;

/// <summary>
/// Represents a reference from a deployment-process' action to a Git resource.
/// </summary>
public class GitDependencyResource
{
    public GitDependencyResource(string name, string repositoryUri, string defaultBranch, 
        string gitCredentialType, string? gitCredentialId = null, string? filePathFilters = null, string? hash = null)
    {
        Name = name;
        RepositoryUri = repositoryUri;
        DefaultBranch = defaultBranch;
        GitCredentialType = gitCredentialType;
        GitCredentialId = gitCredentialId;
        FilePathFilters = filePathFilters;
        Hash = hash;
    }

    public string? Hash { get; }
    public string Name { get; }
    public string RepositoryUri { get; }
    public string DefaultBranch { get; }
    public string? FilePathFilters { get; }
    public string GitCredentialType { get; }
    public string? GitCredentialId { get; }
}