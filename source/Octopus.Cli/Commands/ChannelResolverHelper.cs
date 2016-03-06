using System.Linq;
using log4net;
using Octopus.Cli.Model;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ChannelResolverHelper : IChannelResolverHelper
    {
        readonly ILog log;

        public ChannelResolverHelper(ILog log)
        {
            this.log = log;
        }

        public int GetApplicableStepCount(IOctopusRepository repository, DeploymentProcessResource deploymentProcess, ChannelResource channel, IPackageVersionResolver versionResolver)
        {
            // Need to generate the release plan for this channel, since steps can change depending on channel
            var releaseTemplate = repository.DeploymentProcesses.GetTemplate(deploymentProcess, channel);
            var plan = new ReleasePlan(releaseTemplate, versionResolver);

            // Steps from the release plan are the total number of steps we need to match
            return plan.Steps.Count;
        }

        public bool TestChannelRuleAgainstOctopusApi(IOctopusRepository repository, ChannelResource channel, ChannelVersionRuleResource rule, string packageVersion)
        {
            var checkChannelUri = string.Format("api/channels/rule-test?version={0}&versionRange={1}&preReleaseTag={2}", packageVersion, rule.VersionRange, rule.Tag);
            log.DebugFormat("Channel \"{0}\": Calling Octopus API to test rule => {1}", channel.Name, checkChannelUri);

            var response = repository.Client.Get<OctopusRuleTestResponse>(checkChannelUri);
            log.DebugFormat("Channel \"{0}\": API Response: SatisfiesVersionRang={1} SatisfiesPreReleaseTag={2}", channel.Name, response.SatisfiesVersionRange, response.SatisfiesPreReleaseTag);

            return response.SatisfiesVersionRange && response.SatisfiesPreReleaseTag;
        }
    }
}
