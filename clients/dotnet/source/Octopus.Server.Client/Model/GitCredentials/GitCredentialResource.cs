using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.GitCredentials
{
    public class GitCredentialResource : Resource, INamedResource, IHaveSpaceResource 
    {
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string SpaceId { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public GitCredentialDetails Details { get; set; }
        
        [Writeable]
        public GitCredentialRepositoryRestrictions RepositoryRestrictions { get; set; }

    }
}