using Octopus.Cli.Model;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public interface IChannelVersionRuleTester
    {
        ChannelVersionRuleTestResult Test(IOctopusRepository repository, ChannelVersionRuleResource rule, string packageVersion);
    }
}