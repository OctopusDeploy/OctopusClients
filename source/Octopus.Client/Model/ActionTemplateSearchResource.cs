using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionTemplateSearchResource : IResource
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Website { get; set; }
        public bool IsInstalled { get; set; }
        public bool IsBuiltIn { get; set; }

        public string CommunityActionTemplateId { get; set; }
        public bool HasUpdate { get; set; }

        public LinkCollection Links { get; set; }
    }
}