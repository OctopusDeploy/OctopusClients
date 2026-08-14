using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    class BaseEnvironmentV2Resource : Resource, INamedResource, IHaveSpaceResource, IHaveSlugResource
    {
        public string Name { get; set; }

        public string SpaceId { get; set; }

        public string Slug { get; set; }

        public EnvironmentType Type { get; set; }

        public int SortOrder { get; set; }

        public string Description { get; set; }

        public List<string> EnvironmentTags { get; set; } = new List<string>();
    }
}
