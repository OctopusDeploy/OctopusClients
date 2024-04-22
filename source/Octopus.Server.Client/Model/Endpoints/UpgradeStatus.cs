using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class UpgradeStatus(string value) : CaseInsensitiveStringTinyType(value)
{
    public static UpgradeStatus NoUpgrades => new("NoUpgrades");
    public static UpgradeStatus UpgradeAvailable => new("UpgradeAvailable");
    public static UpgradeStatus UpgradeSuggested => new("UpgradeSuggested");
    public static UpgradeStatus UpgradeRequired => new("UpgradeRequired");
}