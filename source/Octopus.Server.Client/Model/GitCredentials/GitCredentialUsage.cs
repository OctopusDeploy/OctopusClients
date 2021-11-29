using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.GitCredentials
{
    public class GitCredentialUsage
    {
        [Writeable]
        public GitCredentialUsageProject[] Projects { get; set; }

        [Writeable]
        public int OtherProjects { get; set; }
    }

    public class GitCredentialUsageProject
    {
        [Writeable]
        public string ProjectId { get; set; }

        [Writeable]
        public string Slug { get; set; }

        [Writeable]
        public string Name { get; set; }
    }
}