using Octopus.Cli.Model;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ChannelVersionRuleTester : IChannelVersionRuleTester
    {
        public ChannelVersionRuleTestResult Test(IOctopusRepository repository, ChannelVersionRuleResource rule, string packageVersion)
        {
            var link = repository.Client.RootDocument.Link("VersionRuleTest");
            var response = repository.Client.Get<ChannelVersionRuleTestResult>(link, new
            {
                version = packageVersion,
                versionRange = rule.VersionRange,
                preReleaseTag = rule.Tag
            });

            return response;
        }
    }
}
