using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class EventAgentResource : IResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public LinkCollection Links { get; set; }
    }
}
