namespace Octopus.Client.Model
{
    public class DeploymentActionContainerResource
    {
        public string Image { get; set; }
        public string FeedId { get; set; }
        public string GitUrl { get; set; }
        public string Dockerfile { get; set; }
    }
}