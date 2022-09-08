using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model;

public class MachineUpgradeConfigurationResource : IResource
{
    public MachineUpgradeConfigurationResource()
    {
        Id = "machineupgrades";
    }

    [Writeable]
    public int? MachineTaskProcessingLimit { get; set; }

    public string Id { get; }
    public LinkCollection Links { get; set; }
}