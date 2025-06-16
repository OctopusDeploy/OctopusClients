namespace Octopus.Client.Model.GitCredentials;

public class GitCredentialRepositoryRestrictions
{
    public bool Enabled { get; set; }

    public string[] AllowedRepositories { get; set; } = [];
}