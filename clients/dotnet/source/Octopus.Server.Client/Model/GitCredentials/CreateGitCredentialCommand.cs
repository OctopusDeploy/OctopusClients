using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.GitCredentials
{
    public class CreateGitCredentialCommand
    {
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public GitCredentialDetails Details { get; set; }
    }

    internal class CreateGitCredentialResponse
    {
        [Writeable]
        public string Id { get; set; }
    }
}