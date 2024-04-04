using System;

namespace Octopus.Client.Model;

public class LicenseUsageResource: Resource
{
    public SpaceLicenseUsageResource[] SpaceUsage { get; set; } = Array.Empty<SpaceLicenseUsageResource>();
    public int TaskCap { get; set; }
    public LicenseLimitStatusResource[] Limits { get; set; } = new LicenseLimitStatusResource[0];
}
