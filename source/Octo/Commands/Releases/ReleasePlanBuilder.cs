using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Releases
{
    public class ReleasePlanBuilder : IReleasePlanBuilder
    {
        private readonly IPackageVersionResolver versionResolver;
        private readonly IChannelVersionRuleTester versionRuleTester;
        private readonly ICommandOutputProvider commandOutputProvider;

        public ReleasePlanBuilder(ILogger log, IPackageVersionResolver versionResolver, IChannelVersionRuleTester versionRuleTester, ICommandOutputProvider commandOutputProvider)
        {
            this.versionResolver = versionResolver;
            this.versionRuleTester = versionRuleTester;
            this.commandOutputProvider = commandOutputProvider;
        }

        public async Task<ReleasePlan> Build(IOctopusAsyncRepository repository, ProjectResource project, ChannelResource channel, string versionPreReleaseTag)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (project == null) throw new ArgumentNullException(nameof(project));

            commandOutputProvider.Debug("Finding deployment process...");
            var deploymentProcess = await repository.DeploymentProcesses.Get(project.DeploymentProcessId).ConfigureAwait(false);

            commandOutputProvider.Debug("Finding release template...");
            var releaseTemplate = await repository.DeploymentProcesses.GetTemplate(deploymentProcess, channel).ConfigureAwait(false);

            var plan = new ReleasePlan(project, channel, releaseTemplate, deploymentProcess, versionResolver);

            if (plan.UnresolvedSteps.Any())
            {
                commandOutputProvider.Debug("The package version for some steps was not specified. Going to try and resolve those automatically...");
                foreach (var unresolved in plan.UnresolvedSteps)
                {
                    if (!unresolved.IsResolveable)
                    {
                        commandOutputProvider.Error("The version number for step '{Step:l}' cannot be automatically resolved because the feed or package ID is dynamic.", unresolved.ActionName);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(versionPreReleaseTag))
                        commandOutputProvider.Debug("Finding latest package with pre-release '{Tag:l}' for step: {StepName:l}", versionPreReleaseTag, unresolved.ActionName);
                    else
                        commandOutputProvider.Debug("Finding latest package for step: {StepName:l}", unresolved.ActionName);

                    var feed = await repository.Feeds.Get(unresolved.PackageFeedId).ConfigureAwait(false);
                    if (feed == null)
                        throw new CommandException(string.Format("Could not find a feed with ID {0}, which is used by step: " + unresolved.ActionName, unresolved.PackageFeedId));

                    var filters = BuildChannelVersionFilters(unresolved.ActionName, channel);
                    filters["packageId"] = unresolved.PackageId;
                    if (!string.IsNullOrWhiteSpace(versionPreReleaseTag))
                        filters["preReleaseTag"] = versionPreReleaseTag;

                    var packages = await repository.Client.Get<List<PackageResource>>(feed.Link("SearchTemplate"), filters).ConfigureAwait(false);
                    var latestPackage = packages.FirstOrDefault();

                    if (latestPackage == null)
                    {
                        commandOutputProvider.Error("Could not find any packages with ID '{PackageId:l}' in the feed '{FeedUri:l}'", unresolved.PackageId, feed.FeedUri);
                    }
                    else
                    {
                        commandOutputProvider.Debug("Selected '{PackageId:l}' version '{Version:l}' for '{StepName:l}'", latestPackage.PackageId, latestPackage.Version, unresolved.ActionName);
                        unresolved.SetVersionFromLatest(latestPackage.Version);
                    }
                }
            }

            // Test each step in this plan satisfies the channel version rules
            if (channel != null)
            {
                foreach (var step in plan.PackageSteps)
                {
                    // Note the rule can be null, meaning: anything goes
                    var rule = channel.Rules.SingleOrDefault(r => r.Actions.Any(s => s.Equals(step.ActionName, StringComparison.OrdinalIgnoreCase)));
                    var result = await versionRuleTester.Test(repository, rule, step.Version).ConfigureAwait(false);
                    step.SetChannelVersionRuleTestResult(result);
                }
            }

            return plan;
        }

        IDictionary<string, object> BuildChannelVersionFilters(string stepName, ChannelResource channel)
        {
            var filters = new Dictionary<string, object>();

            if (channel == null)
                return filters;

            var rule = channel.Rules.FirstOrDefault(r => r.Actions.Contains(stepName));
            if (rule == null)
                return filters;

            if (!string.IsNullOrWhiteSpace(rule.VersionRange))
                filters["versionRange"] = rule.VersionRange;

            if (!string.IsNullOrWhiteSpace(rule.Tag))
                filters["preReleaseTag"] = rule.Tag;

            return filters;
        }
    }
}