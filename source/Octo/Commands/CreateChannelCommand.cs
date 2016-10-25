using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-channel", Description = "Creates a channel for a project")]
    public class CreateChannelCommand : ApiCommand
    {
        public CreateChannelCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory) : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Create");
            options.Add("project=", "The name of the project in which to create the channel", p => projectName = p);
            options.Add("channel=", "The name of the channel to create", s => channelName = s);
            options.Add("description=", "[Optional] A description of the channel", d => channelDescription = d);
            options.Add("lifecycle=", "[Optional] if specified, the name of the lifecycle to use for promoting releases through this channel, otherwise this channel will inherit the project lifecycle", l => lifecycleName = l);
            options.Add("make-default-channel", "[Optional, Flag] if specified, set the new channel to be the default channel replacing any existing default channel", _ => makeDefaultChannel = true);
            options.Add("update-existing", "[Optional, Flag] if specified, updates the matching channel if it already exists, otherwise this command will fail if a matching channel already exists", _ => updateExisting = true);
        }

        string channelName;
        string projectName;
        string lifecycleName;
        string channelDescription;
        bool updateExisting;
        bool? makeDefaultChannel;

        protected override async Task Execute()
        {
            if (!Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server to start using channels.");
            if (string.IsNullOrWhiteSpace(projectName)) throw new CommandException("Please specify a project using the parameter: --project=ProjectXYZ");
            if (string.IsNullOrWhiteSpace(channelName)) throw new CommandException("Please specify a channel name using the parameter: --channel=ChannelXYZ");

            Log.Debug("Loading project {Project:l}...", projectName);
            var project = await Repository.Projects.FindByName(projectName).ConfigureAwait(false);
            if (project == null) throw new CouldNotFindException("project named", projectName);

            LifecycleResource lifecycle = null;
            if (string.IsNullOrWhiteSpace(lifecycleName))
            {
                Log.Debug("No lifecycle specified. Going to inherit the project lifecycle...");
            }
            else
            {
                Log.Debug("Loading lifecycle {Lifecycle:l}...", lifecycleName);
                lifecycle = await Repository.Lifecycles.FindOne(l => string.Compare(l.Name, lifecycleName, StringComparison.OrdinalIgnoreCase) == 0).ConfigureAwait(false);
                if (lifecycle == null) throw new CouldNotFindException("lifecycle named", lifecycleName);
            }

            var channels = await Repository.Projects.GetChannels(project).ConfigureAwait(false);
            var channel = await channels
                .FindOne(Repository, ch => string.Equals(ch.Name, channelName, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);

            if (channel == null)
            {
                channel = new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = channelName,
                    IsDefault = makeDefaultChannel ?? false,
                    Description = channelDescription ?? string.Empty,
                    LifecycleId = lifecycle?.Id, // Allow for the default lifeycle by propagating null
                    Rules = new List<ChannelVersionRuleResource>(),
                };

                Log.Debug("Creating channel {Channel:l}", channelName);
                await Repository.Channels.Create(channel).ConfigureAwait(false);
                Log.Information("Channel {Channel:l} created", channelName);
                return;
            }

            if (!updateExisting) throw new CommandException("This channel already exists. If you would like to update it, please use the parameter: --update-existing");

            var updateRequired = false;
            if (channel.LifecycleId != lifecycle?.Id)
            {
                if(lifecycle == null)
                    Log.Information("Updating this channel to inherit the project lifecycle for promoting releases");
                else
                    Log.Information("Updating this channel to use lifecycle {Lifecycle:l} for promoting releases", lifecycle.Name);

                channel.LifecycleId = lifecycle?.Id;
                updateRequired = true;
            }

            if (!channel.IsDefault && makeDefaultChannel == true)
            {
                Log.Information("Making this the default channel for {Project:l}", project.Name);
                channel.IsDefault = makeDefaultChannel ?? channel.IsDefault;
                updateRequired = true;
            }

            if (!string.IsNullOrWhiteSpace(channelDescription) && channel.Description != channelDescription)
            {
                Log.Information("Updating channel description to '{Description:l}'", channelDescription);
                channel.Description = channelDescription ?? channel.Description;
                updateRequired = true;
            }

            if (!updateRequired)
            {
                Log.Information("The channel already looks exactly the way it should, no need to update it.");
                return;
            }

            Log.Debug("Updating channel {Channel:l}", channelName);
            await Repository.Channels.Modify(channel).ConfigureAwait(false);
            Log.Information("Channel {Channel:l} updated", channelName);
        }
    }
}