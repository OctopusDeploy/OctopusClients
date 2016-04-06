using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ChannelResolver : IChannelResolver
    {
        readonly ILog log;
        private IChannelResolverHelper helper;
        private List<string> traceInfo;

        public ChannelResolver(ILog log, IChannelResolverHelper helper)
        {
            this.log = log;
            this.helper = helper;
            traceInfo = new List<string>();
        }

        public ChannelResource ResolveByName(string channelName)
        {
            // Find named channel
            var channel = helper.GetChannels().SingleOrDefault(c => string.Equals(c.Name, channelName, StringComparison.OrdinalIgnoreCase));

            if (channel == null)
                throw new CouldNotFindException("a channel named", channelName);

            return channel;
        }

        public ChannelResource ResolveByRules()
        {
            traceInfo.Clear();
            var possibleChannels = new List<ChannelResource>();
            foreach (var channel in helper.GetChannels())
            {
                if (channel.Rules.Count <= 0)
                {
                    Trace("Channel \"{0}\": Skipping due to 0 rules", channel.Name);
                    continue;
                }

                Trace("Channel \"{0}\": Processing {1} rules", channel.Name, channel.Rules.Count);

                // Get the steps required to match
                var requiredMatches = helper.GetApplicableStepCount(channel);
                Trace("Channel \"{0}\": {1} steps need to match channel rules", channel.Name, requiredMatches);

                if (channel.Rules.Sum(r => r.Actions.Count) < requiredMatches)
                {
                    Trace("Channel \"{0}\": Skipping due to rules not covering all required steps", channel.Name);
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

                    Trace("Channel \"{0}\": Checking rule VersionRange:{1} Tag:{2}", channel.Name, versionRange, tag);

                    // For each step this channel rule applies to
                    foreach (var step in rule.Actions)
                    {
                        Trace("Channel \"{0}\": Checking step: {1}", channel.Name, step);

                        // Use version resolver to get the package version for this step
                        string packageVersion = helper.ResolveVersion(channel, step);
                        if (string.IsNullOrEmpty(packageVersion))
                        {
                            Trace("Channel \"{0}\": No version could be resolved for step: {1}", channel.Name, step);
                            continue;
                        }
                        else
                        {
                            Trace("Channel \"{0}\": Resolved version {1} for step: {2}", channel.Name, packageVersion, step);
                        }

                        // Call Octopus URL to validate package version against this rule
                        if (helper.TestChannelRuleAgainstOctopusApi(channel, rule, packageVersion, x => Trace(x)))
                        {
                            stepMatchCount++;
                        }
                        else
                        {
                            stepNoMatchCount++;
                        }
                    }
                }

                Trace("Channel \"{0}\": Matched {1} and failed {2} of {3} required steps", channel.Name, stepMatchCount, stepNoMatchCount, requiredMatches);

                // If any rules failed to match, this channel can't be a candidate
                if (stepNoMatchCount > 0)
                {
                    LogInfo("Channel \"{0}\": Not a viable AutoChannel candidate due to some steps failing version rules", channel.Name);
                    continue;
                }

                // If no rules matched, this channel can't be a candidate
                if (stepMatchCount <= 0)
                {
                    LogInfo("Channel \"{0}\": Not a viable AutoChannel candidate due to no steps matching version rules", channel.Name);
                    continue;
                }

                // Some steps weren't covered by rules at all
                if (stepMatchCount != requiredMatches)
                {
                    LogInfo("Channel \"{0}\": Not a viable AutoChannel candidate due to rules not covering all required steps", channel.Name);
                    continue;
                }

                // All steps have a match
                if (stepMatchCount == requiredMatches)
                {
                    LogInfo("Channel \"{0}\": Matches all steps and is a viable AutoChannel candidate", channel.Name);
                    possibleChannels.Add(channel);
                    continue;
                }
            }

            // Determine if we can match a channel automatically
            var error = "";
            if (possibleChannels.Count > 1)
            {
                // If we have more than one candidate, we can't decide!
                error = string.Format("Could not determine channel automatically as {0} channels ({1}) matched all steps", possibleChannels.Count, string.Join(",", possibleChannels.Select(c => c.Name)));
            }
            else if (possibleChannels.Count == 1)
            {
                // Perfect match for a single channel!
                var channel = possibleChannels.First();
                LogInfo("Channel \"{0}\": Has been automatically selected", channel.Name);
                return channel;
            }
            else
            {
                // Fall back to default
                var defaultChannel = helper.GetChannels().FirstOrDefault(ch => ch.IsDefault);
                if (defaultChannel != null)
                {
                    LogInfo("Channel \"{0}\": Default channel has been automatically selected as no channels were matched", defaultChannel.Name);
                    return defaultChannel;
                }

                error = "Could not determine channel automatically as no channels were matched and no default channel is configured";
            }

            // Something went wrong, log debug level info and throw error
            log.Error(error);
            log.Warn("ResolveChannel Trace:");
            foreach (var line in traceInfo)
            {
                log.Warn(line);
            }
            throw new Exception(string.IsNullOrEmpty(error) ? "Unknown error" : error);
        }

        private void LogInfo(string format, params object[] args)
        {
            log.InfoFormat(format, args);
            Trace(format, args);
        }

        private void Trace(string format, params object[] args)
        {
            traceInfo.Add(string.Format(format, args));
        }
    }
}
