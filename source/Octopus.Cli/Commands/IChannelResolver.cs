using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public interface IChannelResolver
    {
        ChannelResource ResolveByName(string channelName);
        ChannelResource ResolveByRules();
    }
}
