using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class DeploymentActionGitDependencyResource
    {
        public DeploymentActionGitDependencyResource(string deploymentActionSlug)
            : this(deploymentActionSlug, string.Empty)
        {
        }

        [JsonConstructor]
        public DeploymentActionGitDependencyResource(string deploymentActionSlug, string gitDependencyName)
        {
            DeploymentActionSlug = deploymentActionSlug;
            GitDependencyName = gitDependencyName;
        }

        public string DeploymentActionSlug { get; set; }

        public string GitDependencyName { get; set; }
    }
}