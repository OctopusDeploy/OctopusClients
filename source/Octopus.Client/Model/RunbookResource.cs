using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunbookResource : Resource, INamedResource, IHaveSpaceResource
    {
        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        [Trim]
        public string Description { get; set; }

        [Writeable]
        public string RunbookProcessId { get; set; }

        [Writeable]
        public string PublishedRunbookSnapshotId { get; set; }

        [Writeable]
        public string ProjectId { get; set; }

        public string SpaceId { get; set; }
    }
}
