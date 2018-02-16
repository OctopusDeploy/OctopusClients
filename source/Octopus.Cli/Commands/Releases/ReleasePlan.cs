using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Releases
{
    public class ReleasePlan
    {
        readonly ReleasePlanItem[] packageSteps;
        private readonly ReleasePlanItem[] scriptSteps;

        public ReleasePlan(ProjectResource project, ChannelResource channel, ReleaseTemplateResource releaseTemplate, DeploymentProcessResource deploymentProcess, IPackageVersionResolver versionResolver)
        {
            Project = project;
            Channel = channel;
            ReleaseTemplate = releaseTemplate;
            scriptSteps = deploymentProcess.Steps
                .SelectMany(s => s.Actions)
                
                .Select(a => new
                {
                    StepName = a.Name,
                    PackageId = a.Properties.ContainsKey("Octopus.Action.Package.PackageId")
                        ? a.Properties["Octopus.Action.Package.PackageId"].Value
                        : string.Empty,
                    FeedId = a.Properties.ContainsKey("Octopus.Action.Package.FeedId")
                        ? a.Properties["Octopus.Action.Package.FeedId"].Value
                        : string.Empty,
                    a.IsDisabled,
                    a.Channels,

                })
                .Where(x => string.IsNullOrEmpty(x.PackageId) && x.IsDisabled == false) // only consider enabled script steps
                .Where(a => !a.Channels.Any() || a.Channels.Contains(channel.Id)) // only include actions without channel scope or with a matchign channel scope
                .Select(x =>
                    new ReleasePlanItem(x.StepName, null, null, true, null)
                    {
                        IsDisabled = x.IsDisabled
                    }).ToArray();
            packageSteps = releaseTemplate.Packages.Select(
                p => new ReleasePlanItem(
                    p.ActionName,
                    p.PackageId,
                    p.FeedId,
                    p.IsResolvable,
                    versionResolver.ResolveVersion(p.ActionName, p.PackageId)))
                .ToArray();
        }

        public ProjectResource Project { get; }

        public ChannelResource Channel { get; }

        public ReleaseTemplateResource ReleaseTemplate { get; }

        public IEnumerable<ReleasePlanItem> PackageSteps => packageSteps;

        public bool IsViableReleasePlan() => !HasUnresolvedSteps() && !HasStepsViolatingChannelVersionRules() && ChannelHasAnyEnabledSteps();

        public IEnumerable<ReleasePlanItem> UnresolvedSteps
        {
            get
            {
                var releasePlanItems = packageSteps.Where(s => string.IsNullOrWhiteSpace(s.Version));
                return releasePlanItems.ToArray();
            }
        }

        public bool HasUnresolvedSteps()
        {
            return UnresolvedSteps.Any();
        }

        public bool HasStepsViolatingChannelVersionRules()
        {
            return Channel != null && PackageSteps.Any(s => s.ChannelVersionRuleTestResult.IsSatisfied != true);
        }

        public List<SelectedPackage> GetSelections()
        {
            return PackageSteps.Select(x => new SelectedPackage {ActionName = x.ActionName, Version = x.Version}).ToList();
        }

        public string GetHighestVersionNumber()
        {
            var step = PackageSteps.Select(p => SemanticVersion.Parse(p.Version)).OrderByDescending(v => v).FirstOrDefault();
            if (step == null)
            {
                throw new CommandException("None of the deployment packageSteps in this release reference a NuGet package, so the highest package version number cannot be determined.");
            }

            return step.ToString();
        }

        public string FormatAsTable()
        {
            var result = new StringBuilder();
            if (Channel != null)
            {
                result.Append($"Channel: '{Channel.Name}'");
                if (Channel.IsDefault) result.Append(" (this is the default channel)");
                result.AppendLine();
            }

            if (packageSteps.Length == 0)
            {
                return result.ToString();
            }

            var nameColumnWidth = Width("Name", packageSteps.Select(s => s.ActionName));
            var versionColumnWidth = Width("Version", packageSteps.Select(s => s.Version));
            var sourceColumnWidth = Width("Source", packageSteps.Select(s => s.VersionSource));
            var rulesColumnWidth = Width("Version rules", packageSteps.Select(s => s.ChannelVersionRuleTestResult?.ToSummaryString()));
            var format = "  {0,-3} {1,-" + nameColumnWidth + "} {2,-" + versionColumnWidth + "} {3,-" + sourceColumnWidth + "} {4,-" + rulesColumnWidth + "}";

            result.AppendFormat(format, "#", "Name", "Version", "Source", "Version rules").AppendLine();
            result.AppendFormat(format, "---", new string('-', nameColumnWidth), new string('-', versionColumnWidth), new string('-', sourceColumnWidth), new string('-', rulesColumnWidth)).AppendLine();
            for (var i = 0; i < packageSteps.Length; i++)
            {
                var item = packageSteps[i];
                result.AppendFormat(format,
                    i + 1,
                    item.ActionName,
                    item.Version ?? "ERROR",
                    item.VersionSource,
                    item.ChannelVersionRuleTestResult?.ToSummaryString())
                    .AppendLine();
            }

            return result.ToString();
        }

        public int Width(string heading, IEnumerable<string> inputs, int padding = 2, int max = int.MaxValue)
        {
            var calculated = new[] {heading}.Concat(inputs).Where(x => x != null).Max(x => x.Length) + padding;
            return Math.Min(calculated, max);
        }

        public override string ToString()
        {
            return FormatAsTable();
        }

        public string GetActionVersionNumber(string packageStepName)
        {
            var step = packageSteps.SingleOrDefault(s => s.ActionName.Equals(packageStepName, StringComparison.OrdinalIgnoreCase));
            if (step == null)
                throw new CommandException("The step '" + packageStepName + "' is configured to provide the package version number but doesn't exist in the release plan.");
            if (string.IsNullOrWhiteSpace(step.Version))
                throw new CommandException("The step '" + packageStepName + "' provides the release version number but no package version could be determined from it.");
            return step.Version;
        }

        public bool ChannelHasAnyEnabledSteps()
        {
            var channelNotNull = Channel != null;
            
            return packageSteps.AnyEnabled() || scriptSteps.AnyEnabled();
        }
    }

    public static class Extensions
    {
        public static bool AnyEnabled(this IEnumerable<ReleasePlanItem> items)
        {
            if (items == null)
            {
                return false;
            }

            return items.Any(x => x.IsEnabled());
        }

        public static bool IsEnabled(this ReleasePlanItem item)
        {
            return item.IsDisabled == false;
        }
    }
}