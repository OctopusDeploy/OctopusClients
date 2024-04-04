namespace Octopus.Client.Model;

public class SpaceLicenseUsageResource
{
    public string SpaceName { get; set; } = string.Empty;
    public int Projects { get; set; }
    public int Tenants { get; set; }
    public int Machines { get; set; }
}