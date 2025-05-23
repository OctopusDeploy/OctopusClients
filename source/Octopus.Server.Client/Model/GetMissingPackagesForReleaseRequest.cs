namespace Octopus.Client.Model;

public class GetMissingPackagesForReleaseRequest
{
    public string SpaceId { get; set; }

    public string ReleaseId { get; set; }
}