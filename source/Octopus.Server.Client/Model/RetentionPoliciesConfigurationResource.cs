using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RetentionPoliciesConfigurationResource : IResource
    {
        public RetentionPoliciesConfigurationResource()
        {
            Id = "retentionpolicies";
        }

        public string Id { get; set; }

        [Writeable]
        public string CronExpression { get; set; } = "0 */4 * * *";

        public LinkCollection Links { get; set; }
    }
}