namespace Octopus.Client.Model;

public class SpaceLicenseUsageResource
{
    public string SpaceName { get; set; } = string.Empty;
    public int ProjectsCount { get; set; }
    public int TenantsCount { get; set; }
    public int MachinesCount { get; set; }
}