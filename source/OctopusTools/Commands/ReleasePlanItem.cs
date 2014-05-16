using System;

namespace OctopusTools.Commands
{
    public class ReleasePlanItem
    {
        public ReleasePlanItem(string stepName, string packageId, string nuGetFeedId, bool isResolveable, string userSpecifiedVersion)
        {
            StepName = stepName;
            PackageId = packageId;
            NuGetFeedId = nuGetFeedId;
            IsResolveable = isResolveable;
            Version = userSpecifiedVersion;
            VersionSource = string.IsNullOrWhiteSpace(Version) ? string.Empty : "User specified";
        }

        public string StepName { get; set; }

        public string PackageId { get; set; }

        public string Version { get; set; }

        public string NuGetFeedId { get; set; }

        public bool IsResolveable { get; set; }

        public string VersionSource { get; private set; }

        public void SetVersionFromLatest(string version)
        {
            Version = version;
            VersionSource = "Latest available in NuGet repository";
        }
    }
}