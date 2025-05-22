using System.Collections.Generic;

namespace Octopus.Client.Model;

public class GetMissingPackagesForReleaseResponse
{
    public IReadOnlyCollection<MissingPackageInfo> Packages { get; set; }
}

public class MissingPackageInfo
{
    public string Id { get; set; }

    public string Version { get; set; }
}