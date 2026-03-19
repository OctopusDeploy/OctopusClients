namespace Octopus.Client.Model;

public class InterruptionPullRequestResource
{
    public string Id { get; set; }
    
    public string InterruptionId { get; set; }
    public string RepositoryUrl { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public long Number { get; set; }
    public InterruptionPullRequestStatus Status { get; set; }
}
