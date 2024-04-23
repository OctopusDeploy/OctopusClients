using System;

namespace Octopus.Client.Model;
public class LicenseUsageResource: Resource
{
    public SpaceLicenseUsageResource[] SpacesUsage { get; set; } = Array.Empty<SpaceLicenseUsageResource>();
    public LicenseLimitUsageResource[] Limits { get; set; } = new LicenseLimitUsageResource[0];
}