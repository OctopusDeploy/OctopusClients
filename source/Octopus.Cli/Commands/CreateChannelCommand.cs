using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
            options.Add("channel=", "The name of the channel to create", s => channelName = s);
            options.Add("description=", "A description of the channel", d => channelDescription = d);
            options.Add("project=", "The name of the project in which to create the channel", p => projectName = p);
            options.Add("lifecycle=", "The name of the lifecycle with which to create the channel", l => lifecycleName = l);
            options.Add("make-default-channel", "Set the new channel to be the default channel", _ => makeDefaultChannel = true);
            options.Add("update-if-exists", "Updates the channel if it already exists", _ => updateIfExists = true);
        }

        string channelName;
        string projectName;
        string lifecycleName;
        string channelDescription;
        bool updateIfExists;
        bool? makeDefaultChannel;

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(projectName)) throw new CommandException("Please specify a project using the parameter: --project=ProjectXYZ");
            if (string.IsNullOrWhiteSpace(channelName)) throw new CommandException("Please specify a channel name using the parameter: --channel=ChannelXYZ");

            Log.DebugFormat("Loading project {0}", projectName);
            var project = Repository.Projects.FindByName(projectName);
            if (project == null) throw new CouldNotFindException("project named", projectName);

            string lifecycleId = null;
            if (string.IsNullOrWhiteSpace(lifecycleName))
            {
                Log.DebugFormat("No lifecycle specified. Using default.");
            }
            else
            {
                Log.DebugFormat("Loading lifecycle {0}", lifecycleName);
                var lifecycle = Repository.Lifecycles.FindOne(l => string.Compare(l.Name, lifecycleName, StringComparison.OrdinalIgnoreCase) == 0);
                if (lifecycle == null) throw new CouldNotFindException("lifecycle named", lifecycleName);
                lifecycleId = lifecycle.Id;
            }

            var channelsForThisProject = Repository.Client.List<ChannelResource>(project.Links["Channels"]);
            var channel = channelsForThisProject.Items.FirstOrDefault(ch => string.Compare(ch.Name, channelName, StringComparison.OrdinalIgnoreCase) == 0);

            if (channel == null)
            {
                channel = new ChannelResource()
                {
                    ProjectId = project.Id,
                    Name = channelName,
                    IsDefault = makeDefaultChannel ?? false,
                    Description = channelDescription ?? string.Empty,
                    LifecycleId = lifecycleId,
                    Rules = new List<ChannelVersionRuleResource>(),
                };

                Log.DebugFormat("Creating channel {0}", channelName);
                Repository.Channels.Create(channel);
                Log.InfoFormat("Channel {0} created", channelName);
            }
            else
            {
                if (!updateIfExists) throw new CommandException("This channel already exists. If you would like to update it, please use the parameter: --update-if-exists");

                channel.LifecycleId = lifecycleId ?? channel.LifecycleId;
                channel.IsDefault = makeDefaultChannel ?? channel.IsDefault;
                channel.Description = channelDescription ?? channel.Description;

                Log.DebugFormat("Updating channel {0}", channelName);
                Repository.Channels.Modify(channel);
                Log.InfoFormat("Channel {0} updated", channelName);
            }
        }
    }
}