using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    public class ReleasePlan
    {
        readonly IList<ReleasePlanItem> steps = new List<ReleasePlanItem>();

        public ReleasePlan(ReleaseTemplateResource releaseTemplate, IPackageVersionResolver versionResolver)
        {       
            steps.AddRange(
                releaseTemplate.Packages.Select(p => new ReleasePlanItem(
                    p.StepName,
                    p.NuGetPackageId,
                    p.NuGetFeedId,
                    p.IsResolvable,
                    versionResolver.ResolveVersion(p.StepName) ?? versionResolver.ResolveVersion(p.NuGetPackageId)
                    ))
                );
        }

        public IList<ReleasePlanItem> Steps
        {
            get { return steps; }
        }

        public IList<ReleasePlanItem> UnresolvedSteps
        {
            get { return steps.Where(s => string.IsNullOrWhiteSpace(s.Version)).ToList(); }
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

            if (steps.Count == 0)
            {
                return string.Empty;
            }

            var nameColumnWidth = Math.Min(steps.Max(s => s.StepName.Length) + 2, 30);
            var format = "  {0,-3} {1,-" + nameColumnWidth + "} {2,-15} {3,-36}";

            result.AppendFormat(format, "#", "Name", "Version", "Source").AppendLine();
            result.AppendFormat(format, "---", new string('-', nameColumnWidth), new string('-', 15), new string('-', 36)).AppendLine();
            for (int i = 0; i < steps.Count; i++)
            {
                var item = steps[i];
                result.AppendFormat(format, i + 1, item.StepName, item.Version ?? "ERROR", string.IsNullOrWhiteSpace(item.VersionSource) ? "Cannot resolve" : item.VersionSource).AppendLine();
            }

            return result.ToString();
        }

        public override string ToString()
        {
            return FormatAsTable();
        }

        public bool HasUnresolvedSteps()
        {
            return UnresolvedSteps.Count > 0;
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