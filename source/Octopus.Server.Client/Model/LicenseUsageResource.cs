using System;

namespace Octopus.Client.Model;
public class LicenseUsageResource: Resource
{
    public SpaceLicenseUsageResource[] SpacesUsage { get; set; } = Array.Empty<SpaceLicenseUsageResource>();
    public LicenseLimitStatusResource[] Limits { get; set; } = new LicenseLimitStatusResource[0];
}