using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ScheduledTaskStatusResource : Resource, INamedResource
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
    }
}