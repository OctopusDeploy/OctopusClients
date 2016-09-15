using System;

namespace Octopus.Client.Model
{
    public class StepResource : IResource
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool IsInstalled { get; set; }

        public LinkCollection Links { get; set; }
    }
}