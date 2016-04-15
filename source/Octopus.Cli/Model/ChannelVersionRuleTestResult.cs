using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Cli.Model
{
    public class ChannelVersionRuleTestResult : Resource
    {
        public IEnumerable<string> Errors { get; set; }
        public bool SatisfiesVersionRange { get; set; }
        public bool SatisfiesPreReleaseTag { get; set; }
        public bool IsSatisfied => SatisfiesVersionRange && SatisfiesPreReleaseTag;

        const string Pass = "PASS";
        const string Fail = "FAIL";

        public string ToSummaryString()
        {
            return $"Range: {(SatisfiesVersionRange ? Pass : Fail)} Tag: {(SatisfiesPreReleaseTag ? Pass : Fail)}";
        }
    }
}
