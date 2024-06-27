using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ChannelGitResourceRuleResource
    {
        public string Id { get; set; }
        public List<DeploymentActionGitDependencyResource> GitDependencyActions { get; set; }
        public List<string> Rules { get; set; }
    }
}