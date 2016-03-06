using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ChannelResolver : IChannelResolver
    {
        readonly ILog log;
        private List<ChannelResource> channels;
        private DeploymentProcessResource deploymentProcess;
        private IChannelResolverHelper helper;

        public ChannelResolver(ILog log, IChannelResolverHelper helper, IPackageVersionResolver versionResolver)
        {
            this.log = log;
            this.channels = new List<ChannelResource>();
            this.helper = helper;
        }

        public void RegisterProject(ProjectResource project, IOctopusRepository repository)
        {
            RegisterChannels(repository.Projects.GetChannels(project).Items);
            RegisterDeploymentProcess(repository.DeploymentProcesses.Get(project.DeploymentProcessId));
        }

        public void RegisterChannels(IEnumerable<ChannelResource> channels)
        {
            this.channels.AddRange(channels);
        }

        public void RegisterDeploymentProcess(DeploymentProcessResource process)
        {
            this.deploymentProcess = process;
        }

        public ChannelResource ResolveByName(string channelName)
        {
            // Find named channel
            var channel = channels.SingleOrDefault(c => string.Equals(c.Name, channelName, StringComparison.OrdinalIgnoreCase));

            if (channel == null)
                throw new CouldNotFindException("a channel named", channelName);

            return channel;
        }

        public ChannelResource ResolveByRules(IOctopusRepository repository, IPackageVersionResolver versionResolver)
        {
            var possibleChannels = new List<ChannelResource>();
            foreach (var channel in channels)
            {
                if (channel.Rules.Count <= 0)
                {
                    log.DebugFormat("Channel \"{0}\": Skipping due to 0 rules", channel.Name);
                    continue;
                }

                log.DebugFormat("Channel \"{0}\": Processing {1} rules", channel.Name, channel.Rules.Count);

                // Get the steps required to match
                var requiredMatches = helper.GetApplicableStepCount(repository, deploymentProcess, channel, versionResolver);
                log.DebugFormat("Channel \"{0}\": {1} steps need to match channel rules", channel.Name, requiredMatches);

                if (channel.Rules.Sum(r => r.Actions.Count) < requiredMatches)
                {
                    log.DebugFormat("Channel \"{0}\": Skipping due to rules not covering all required steps", channel.Name);
                    continue;
                }

                var stepMatchCount = 0;
                var stepNoMatchCount = 0;

                // Check channel rules for channel
                foreach (var rule in channel.Rules)
                {
                    // Get the channel rule details
                    string versionRange = rule.VersionRange;
                    string tag = rule.Tag;

                    log.DebugFormat("Channel \"{0}\": Checking rule VersionRange:{1} Tag:{2}", channel.Name, versionRange, tag);

                    // For each step this channel rule applies to
                    foreach (var step in rule.Actions)
                    {
                        log.DebugFormat("Channel \"{0}\": Checking step: {1}", channel.Name, step);

                        // Use version resolver to get the package version for this step
                        string packageVersion = versionResolver.ResolveVersion(step);
                        if (string.IsNullOrEmpty(packageVersion))
                        {
                            log.DebugFormat("Channel \"{0}\": No version could be resolved for step: {1}", channel.Name, step);
                            continue;
                        }
                        else
                        {
                            log.DebugFormat("Channel \"{0}\": Resolved version {1} for step: {2}", channel.Name, packageVersion, step);
                        }

                        // Call Octopus URL to validate package version against this rule
                        if (helper.TestChannelRuleAgainstOctopusApi(repository, channel, rule, packageVersion))
                        {
                            stepMatchCount++;
                        }
                        else
                        {
                            stepNoMatchCount++;
                        }
                    }
                }

                log.DebugFormat("Channel \"{0}\": Matched {1} and failed {2} of {3} required steps", channel.Name, stepMatchCount, stepNoMatchCount, requiredMatches);

                // If any rules failed to match, this channel can't be a candidate
                if (stepNoMatchCount > 0)
                {
                    log.InfoFormat("Channel \"{0}\": Not a viable AutoChannel candidate due to some steps failing version rules", channel.Name);
                    continue;
                }

                // If no rules matched, this channel can't be a candidate
                if (stepMatchCount <= 0)
                {
                    log.InfoFormat("Channel \"{0}\": Not a viable AutoChannel candidate due to no steps matching version rules", channel.Name);
                    continue;
                }

                // Some steps weren't covered by rules at all
                if (stepMatchCount != requiredMatches)
                {
                    log.InfoFormat("Channel \"{0}\": Not a viable AutoChannel candidate due to rules not covering all required steps", channel.Name);
                    continue;
                }

                // All steps have a match
                if (stepMatchCount == requiredMatches)
                {
                    log.InfoFormat("Channel \"{0}\": Matches all steps and is a viable AutoChannel candidate", channel.Name);
                    possibleChannels.Add(channel);
                    continue;
                }
            }

            // If we have more than one candidate, we can't decide!
            if (possibleChannels.Count > 1)
            {
                var message = string.Format("Could not determine channel automatically as {0} channels ({1}) matched all steps", possibleChannels.Count, string.Join(",", possibleChannels.Select(c => c.Name)));
                throw new Exception(message);
            }
            else if (possibleChannels.Count == 1)
            {
                var channel = possibleChannels.First();
                log.InfoFormat("Channel \"{0}\": Has been automatically selected", channel.Name);
                return channel;
            }
            else
            {
                // Fall back to default
                var defaultChannel = channels.FirstOrDefault(ch => ch.IsDefault);
                if (defaultChannel != null)
                {
                    log.InfoFormat("Channel \"{0}\": Default channel has been automatically selected as no channels were matched", defaultChannel.Name);
                    return defaultChannel;
                }

                var message = "Could not determine channel automatically as no channels were matched and no default channel is configured";
                throw new Exception(message);
            }
        }
    }
}
