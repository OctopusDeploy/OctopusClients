using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public class ReleasePlan
    {
        readonly ReleasePlanItem[] steps;

        public ReleasePlan(ProjectResource project, ChannelResource channel, ReleaseTemplateResource releaseTemplate, IPackageVersionResolver versionResolver)
        {
            Project = project;
            Channel = channel;
            ReleaseTemplate = releaseTemplate;
            steps = releaseTemplate.Packages.Select(
                p => new ReleasePlanItem(
                    p.StepName,
                    p.PackageId,
                    p.FeedId,
                    p.IsResolvable,
                    versionResolver.ResolveVersion(p.StepName, p.PackageId)))
                .ToArray();
        }

        public ProjectResource Project { get; }

        public ChannelResource Channel { get; }

        public ReleaseTemplateResource ReleaseTemplate { get; }

        public IEnumerable<ReleasePlanItem> Steps => steps;

        public bool IsViableReleasePlan() => !HasUnresolvedSteps() && !HasStepsViolatingChannelVersionRules();

        public IEnumerable<ReleasePlanItem> UnresolvedSteps
        {
            get { return steps.Where(s => string.IsNullOrWhiteSpace(s.Version)).ToArray(); }
        }

        public bool HasUnresolvedSteps()
        {
            return UnresolvedSteps.Any();
        }

        public bool HasStepsViolatingChannelVersionRules()
        {
            return Channel != null && Steps.Any(s => s.ChannelVersionRuleTestResult.IsSatisfied != true);
        }

        public List<SelectedPackage> GetSelections()
        {
            return Steps.Select(x => new SelectedPackage {StepName = x.StepName, Version = x.Version}).ToList();
        }

        public string GetHighestVersionNumber()
        {
            var step = Steps.Select(p => SemanticVersion.Parse(p.Version)).OrderByDescending(v => v).FirstOrDefault();
            if (step == null)
            {
                throw new CommandException("None of the deployment steps in this release reference a NuGet package, so the highest package version number cannot be determined.");
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

            if (steps.Length == 0)
            {
                return result.ToString();
            }

            var nameColumnWidth = Width("Name", steps.Select(s => s.StepName));
            var versionColumnWidth = Width("Version", steps.Select(s => s.Version));
            var sourceColumnWidth = Width("Source", steps.Select(s => s.VersionSource));
            var rulesColumnWidth = Width("Version rules", steps.Select(s => s.ChannelVersionRuleTestResult?.ToSummaryString()));
            var format = "  {0,-3} {1,-" + nameColumnWidth + "} {2,-" + versionColumnWidth + "} {3,-" + sourceColumnWidth + "} {4,-" + rulesColumnWidth + "}";

            result.AppendFormat(format, "#", "Name", "Version", "Source", "Version rules").AppendLine();
            result.AppendFormat(format, "---", new string('-', nameColumnWidth), new string('-', versionColumnWidth), new string('-', sourceColumnWidth), new string('-', rulesColumnWidth)).AppendLine();
            for (var i = 0; i < steps.Length; i++)
            {
                var item = steps[i];
                result.AppendFormat(format,
                    i + 1,
                    item.StepName,
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
            var step = steps.SingleOrDefault(s => s.StepName.Equals(packageStepName, StringComparison.OrdinalIgnoreCase));
            if (step == null)
                throw new CommandException("The step '" + packageStepName + "' is configured to provide the package version number but doesn't exist in the release plan.");
            if (string.IsNullOrWhiteSpace(step.Version))
                throw new CommandException("The step '" + packageStepName + "' provides the release version number but no package version could be determined from it.");
            return step.Version;
        }
    }
}