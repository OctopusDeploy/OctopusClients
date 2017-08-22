using Octopus.Client.Model;

namespace Octopus.Cli.Model
{
    public class DeploymentRelatedResources
    {
        public ChannelResource ChannelResource { get; set; }
        public ReleaseResource ReleaseResource { get; set; }
    }
}