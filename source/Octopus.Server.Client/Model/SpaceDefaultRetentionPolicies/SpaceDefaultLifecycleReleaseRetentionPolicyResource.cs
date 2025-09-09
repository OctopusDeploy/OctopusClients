#nullable enable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    public class SpaceDefaultLifecycleReleaseRetentionPolicyResource: SpaceDefaultRetentionPolicyResource
    {
        public SpaceDefaultLifecycleReleaseRetentionPolicyResource(RetentionPeriodStrategy strategy)
        {
            Strategy = strategy;
        }

        public override RetentionType RetentionType => RetentionType.LifecycleRelease;
        
        [Description("Retention strategy for the default lifecycle release retention policy. ['Count', 'Forever']")]
        [Writeable]
        [Required]
        public RetentionPeriodStrategy Strategy { get; set; }
        
        [Description("Quantity of releases to keep for the default lifecycle release retention policy.")]
        [Writeable]
        public int? QuantityToKeep { get; set; }
        
        [Description("Unit of measurement for the quantity of releases to keep. ['Days', 'Items']")]
        [Writeable]
        public LifecycleReleaseRetentionUnit? Unit { get; set; }
        
    }
}
