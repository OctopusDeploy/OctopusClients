using System;
using Octopus.Cli.Model;

namespace Octopus.Cli.Commands.Releases
{
    public class ReleasePlanItem
    {
        public ReleasePlanItem(string actionName, string packageId, string packageFeedId, bool isResolveable, string userSpecifiedVersion)
        {
            ActionName = actionName;
            PackageId = packageId;
            PackageFeedId = packageFeedId;
            IsResolveable = isResolveable;
            Version = userSpecifiedVersion;
            VersionSource = string.IsNullOrWhiteSpace(Version) ? "Cannot resolve" : "User specified";
        }

        public string ActionName { get; }

        public string PackageId { get; }

        public string PackageFeedId { get; }

        public bool IsResolveable { get; }

        public string Version { get; private set; }

        public string VersionSource { get; private set; }

        public ChannelVersionRuleTestResult ChannelVersionRuleTestResult { get; private set; }

        public void SetVersionFromLatest(string version)
        {
            Version = version;
            VersionSource = "Latest available";
        }

        public void SetChannelVersionRuleTestResult(ChannelVersionRuleTestResult result)
        {
            ChannelVersionRuleTestResult = result;
        }

        public bool IsDisabled { get; set; }
    }
}