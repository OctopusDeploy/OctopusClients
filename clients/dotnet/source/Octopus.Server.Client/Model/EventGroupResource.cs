using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    /// <summary>
    /// To help represent our EventGroup enum to help with event filtering.
    /// </summary>
    public class EventGroupResource : IResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> EventCategories { get; set; }
        public LinkCollection Links { get; set; }
    }
}
