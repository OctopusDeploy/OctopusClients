namespace Octopus.Client.Model.Endpoints;

public record KubernetesAgentUpgradeInformationResource
{
    public UpgradeStatusResource UpgradeStatus { get; set; } = UpgradeStatusResource.NoUpgrades;

    public bool UpgradeLocked { get; set; }
}