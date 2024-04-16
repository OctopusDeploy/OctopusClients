using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class UpgradeStatusResource : CaseInsensitiveStringTinyType
{
    public static UpgradeStatusResource NoUpgrades => new("NoUpgrades");
    public static UpgradeStatusResource UpgradeAvailable => new("UpgradeAvailable");
    public static UpgradeStatusResource UpgradeSuggested => new("UpgradeSuggested");
    public static UpgradeStatusResource UpgradeRequired => new("UpgradeRequired");

    public UpgradeStatusResource(string value) : base(value)
    {
    }
}