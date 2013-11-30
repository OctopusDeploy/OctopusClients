using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Octopus.Client.Model;

namespace OctopusTools.Commands
{
    public class ReleasePlan
    {
        private readonly IList<ReleasePlanItem> steps;

        public ReleasePlan(IEnumerable<DeploymentStepResource> deploymentSteps, IPackageVersionResolver versionResolver)
        {
            foreach (var step in deploymentSteps)
            {
                steps = step.Actions
                    .Where(x => x.ActionType == "Octopus.TentaclePackage")
                    .Select(a => new ReleasePlanItem(a.Name, a.Properties["Octopus.Action.Package.NuGetPackageId"], a.Properties["Octopus.Action.Package.NuGetFeedId"], versionResolver.ResolveVersion(a.Properties["Octopus.Action.Package.NuGetPackageId"])))
                    .ToList();
            }
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
                throw new ArgumentException("None of the deployment steps in this release reference a NuGet package, so the highest package version number cannot be determined.");
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
                result.AppendFormat(format, i + 1, item.StepName, item.Version, item.VersionSource).AppendLine();
            }

            return result.ToString();
        }

        public override string ToString()
        {
            return FormatAsTable();
        }
    }
}