using System;
using System.Collections.Generic;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-channel", Description = "Creates a channel for a project")]
    public class CreateChannelCommand : ApiCommand
    {
        public CreateChannelCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem) : base(repositoryFactory, log, fileSystem)
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

        protected override void Execute()
        {
            if (!Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server to start using channels.");
            if (string.IsNullOrWhiteSpace(projectName)) throw new CommandException("Please specify a project using the parameter: --project=ProjectXYZ");
            if (string.IsNullOrWhiteSpace(channelName)) throw new CommandException("Please specify a channel name using the parameter: --channel=ChannelXYZ");

            Log.DebugFormat("Loading project {0}...", projectName);
            var project = Repository.Projects.FindByName(projectName);
            if (project == null) throw new CouldNotFindException("project named", projectName);

            LifecycleResource lifecycle = null;
            if (string.IsNullOrWhiteSpace(lifecycleName))
            {
                Log.DebugFormat("No lifecycle specified. Going to inherit the project lifecycle...");
            }
            else
            {
                Log.DebugFormat("Loading lifecycle {0}...", lifecycleName);
                lifecycle = Repository.Lifecycles.FindOne(l => string.Compare(l.Name, lifecycleName, StringComparison.OrdinalIgnoreCase) == 0);
                if (lifecycle == null) throw new CouldNotFindException("lifecycle named", lifecycleName);
            }

            var channel = Repository.Projects.GetChannels(project)
                .FindOne(Repository, ch => string.Equals(ch.Name, channelName, StringComparison.OrdinalIgnoreCase));

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

                Log.DebugFormat("Creating channel {0}", channelName);
                Repository.Channels.Create(channel);
                Log.InfoFormat("Channel {0} created", channelName);
                return;
            }

            if (!updateExisting) throw new CommandException("This channel already exists. If you would like to update it, please use the parameter: --update-existing");

            var updateRequired = false;
            if (channel.LifecycleId != lifecycle?.Id)
            {
                Log.InfoFormat("Updating this channel to {0}", lifecycle != null ? $"use lifecycle {lifecycle.Name} for promoting releases" : "inherit the project lifecycle for promoting releases");
                channel.LifecycleId = lifecycle?.Id;
                updateRequired = true;
            }

            if (!channel.IsDefault && makeDefaultChannel == true)
            {
                Log.InfoFormat("Making this the default channel for {0}", project.Name);
                channel.IsDefault = makeDefaultChannel ?? channel.IsDefault;
                updateRequired = true;
            }

            if (!string.IsNullOrWhiteSpace(channelDescription) && channel.Description != channelDescription)
            {
                Log.InfoFormat("Updating channel description to '{0}'", channelDescription);
                channel.Description = channelDescription ?? channel.Description;
                updateRequired = true;
            }

            if (!updateRequired)
            {
                Log.InfoFormat("The channel already looks exactly the way it should, no need to update it.");
                return;
            }

            Log.DebugFormat("Updating channel {0}", channelName);
            Repository.Channels.Modify(channel);
            Log.InfoFormat("Channel {0} updated", channelName);
        }
    }
}