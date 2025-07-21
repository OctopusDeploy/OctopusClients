using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    /// <summary>
    /// To help represent our EventCategory enum to help with event filtering.
    /// </summary>
    public class EventCategoryResource : IResource, INamedResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public LinkCollection Links { get; set; }
    }
}
