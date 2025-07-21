using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RetentionPoliciesConfigurationResource : Resource
    {
        public RetentionPoliciesConfigurationResource()
        {
            Id = "retentionpolicies";
        }

        [Writeable]
        public string CronExpression { get; set; } = "0 */4 * * *";
    }
}