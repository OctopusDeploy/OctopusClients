using Octopus.Cli.Model;

namespace Octopus.Cli.Commands
{
    public class ReleasePlanItem
    {
        public ReleasePlanItem(string stepName, string packageId, string packageFeedId, bool isResolveable, string userSpecifiedVersion)
        {
            StepName = stepName;
            PackageId = packageId;
            PackageFeedId = packageFeedId;
            IsResolveable = isResolveable;
            Version = userSpecifiedVersion;
            VersionSource = string.IsNullOrWhiteSpace(Version) ? "Cannot resolve" : "User specified";
        }

        public string StepName { get; }

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
    }
}