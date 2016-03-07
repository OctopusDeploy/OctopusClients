using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public interface IChannelResolverHelper
    {
        void SetContext(IOctopusRepository repository, ProjectResource project);
        IEnumerable<ChannelResource> GetChannels();
        int GetApplicableStepCount(ChannelResource channel);
        bool TestChannelRuleAgainstOctopusApi(ChannelResource channel, ChannelVersionRuleResource rule, string packageVersion);
        string ResolveVersion(ChannelResource channel, string step);
    }
}