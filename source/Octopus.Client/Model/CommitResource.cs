namespace Octopus.Client.Model
{
    public class CommitResource<TResource> where TResource : Resource
    {
        public TResource Resource { get; set; }
        public string CommitMessage { get; set; }
    }
}