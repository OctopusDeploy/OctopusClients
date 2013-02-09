using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctopusTools.Model;

namespace OctopusTools.Commands
{
    public class ReleasePlan
    {
        private readonly IList<ReleasePlanItem> steps; 

        public ReleasePlan(IEnumerable<Step> steps, IPackageVersionResolver versionResolver)
        {
            this.steps = steps.Select(delegate(Step s)
            {
                var version = versionResolver.ResolveVersion(s.ToString())
                    ?? versionResolver.ResolveVersion(s.NuGetPackageId);
                return new ReleasePlanItem(s, version);
            }).ToList();
        }

        public IList<ReleasePlanItem> Steps
        {
            get { return steps; }
        }

        public IList<ReleasePlanItem> UnresolvedSteps
        {
            get { return steps.Where(s => string.IsNullOrWhiteSpace(s.Version)).ToList(); }
        }

        public IList<SelectedPackage> GetSelections()
        {
            return Steps.Select(s => new SelectedPackage {StepId = s.Step.Id, NuGetPackageVersion = s.Version}).ToList();
        } 

        public string GetHighestVersionNumber()
        {
            return Steps.Select(p => SemanticVersion.Parse(p.Version)).OrderByDescending(v => v).First().ToString();
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