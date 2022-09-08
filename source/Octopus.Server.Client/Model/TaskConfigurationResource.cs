using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model;

public class TaskConfigurationResource : IResource
{
    public TaskConfigurationResource()
    {
        Id = "task";
    }

    [Writeable]
    public int? MachineTaskProcessingLimit { get; set; }

    public string Id { get; }
    public LinkCollection Links { get; set; }
}