using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;

namespace OctopusTools.Commands
{
    public interface IChannelResolver
    {
        void RegisterProject(ProjectResource project, IOctopusRepository repository);
        void RegisterChannels(IEnumerable<ChannelResource> channels);
        void RegisterDeploymentProcess(DeploymentProcessResource process);
        ChannelResource ResolveByName(string channelName);
        ChannelResource ResolveByRules(IOctopusRepository repository, IPackageVersionResolver versionResolver);
    }
}
