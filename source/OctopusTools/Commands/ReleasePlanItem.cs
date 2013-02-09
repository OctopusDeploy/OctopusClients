using System;
using OctopusTools.Model;

namespace OctopusTools.Commands
{
    public class ReleasePlanItem
    {
        public ReleasePlanItem(Step step, string userSpecifiedVersion)
        {
            Step = step;
            Version = userSpecifiedVersion;
            VersionSource = string.IsNullOrWhiteSpace(Version) ? string.Empty : "User specified";
        }

        public Step Step { get; private set; }
        public string StepName { get { return Step.ToString(); }}
        public string VersionSource { get; private set; }
        public string Version { get; private set; }

        public void SetVersionFromLatest(string version)
        {
            Version = version;
            VersionSource = "Latest available in NuGet repository";
        }
    }
}