using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.ArchiveLimits
{
    public class ArchiveLimitConfigurationResource : IResource
    {
        public ArchiveLimitConfigurationResource()
        {
            Id = "archive-limit";
        }

        public string Id { get; set; }
        
        [Writeable]
        public long OctopusServerGigabytes { get; set; }

        [Writeable]
        public long DeploymentPackageGigabytes { get; set; }

        public LinkCollection Links { get; set; }
    }
}