using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ReleasePlanBuilder : IReleasePlanBuilder
    {
        private readonly ILogger log;
        private readonly IPackageVersionResolver versionResolver;
        private readonly IChannelVersionRuleTester versionRuleTester;

        public ReleasePlanBuilder(ILogger log, IPackageVersionResolver versionResolver, IChannelVersionRuleTester versionRuleTester)
        {
            this.log = log;
            this.versionResolver = versionResolver;
            this.versionRuleTester = versionRuleTester;
        }

        public ReleasePlan Build(IOctopusRepository repository, ProjectResource project, ChannelResource channel, string versionPreReleaseTag)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (project == null) throw new ArgumentNullException(nameof(project));

            log.Debug("Finding deployment process...");
            var deploymentProcess = repository.DeploymentProcesses.Get(project.DeploymentProcessId);

            log.Debug("Finding release template...");
            var releaseTemplate = repository.DeploymentProcesses.GetTemplate(deploymentProcess, channel);

            var plan = new ReleasePlan(project, channel, releaseTemplate, versionResolver);

            if (plan.UnresolvedSteps.Any())
            {
                log.Debug("The package version for some steps was not specified. Going to try and resolve those automatically...");
                foreach (var unresolved in plan.UnresolvedSteps)
                {
                    if (!unresolved.IsResolveable)
                    {
                        log.Error("The version number for step '{0}' cannot be automatically resolved because the feed or package ID is dynamic.", unresolved.StepName);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(versionPreReleaseTag))
                        log.Debug("Finding latest package with pre-release '{1}' for step: {0}", unresolved.StepName, versionPreReleaseTag);
                    else
                        log.Debug("Finding latest package for step: {0}", unresolved.StepName);

                    var feed = repository.Feeds.Get(unresolved.PackageFeedId);
                    if (feed == null)
                        throw new CommandException(string.Format("Could not find a feed with ID {0}, which is used by step: " + unresolved.StepName, unresolved.PackageFeedId));

                    var filters = BuildChannelVersionFilters(unresolved.StepName, channel);
                    filters["packageId"] = unresolved.PackageId;
                    if (!string.IsNullOrWhiteSpace(versionPreReleaseTag))
                        filters["preReleaseTag"] = versionPreReleaseTag;

                    var packages = repository.Client.Get<List<PackageResource>>(feed.Link("SearchTemplate"), filters);
                    var latestPackage = packages.FirstOrDefault();

                    if (latestPackage == null)
                    {
                        log.Error("Could not find any packages with ID '{0}' in the feed '{1}'", unresolved.PackageId, feed.FeedUri);
                    }
                    else
                    {
                        log.Debug("Selected '{0}' version '{1}' for '{2}'", latestPackage.NuGetPackageId, latestPackage.Version, unresolved.StepName);
                        unresolved.SetVersionFromLatest(latestPackage.Version);
                    }
                }
            }

            // Test each step in this plan satisfies the channel version rules
            if (channel != null)
            {
                foreach (var step in plan.Steps)
                {
                    // Note the rule can be null, meaning: anything goes
                    var rule = channel.Rules.SingleOrDefault(r => r.Actions.Any(s => s.Equals(step.StepName, StringComparison.OrdinalIgnoreCase)));
                    var result = versionRuleTester.Test(repository, rule, step.Version);
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