namespace Octopus.Client.Model
{
    public class CommitResource
    {
        public string CommitMessage { get; set; }
    }

    public class CommitResource<TResource> : CommitResource where TResource : Resource
    {
        public TResource Resource { get; set; }
    }
}