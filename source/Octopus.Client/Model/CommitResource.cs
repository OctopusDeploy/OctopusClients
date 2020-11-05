namespace Octopus.Client.Model
{
    public class CommitResource
    {
        public string CommitMessage { get; set; }
    }

    public class CommitResource<TResource> : CommitResource where TResource : class
    {
        public TResource Resource { get; set; }
    }
}