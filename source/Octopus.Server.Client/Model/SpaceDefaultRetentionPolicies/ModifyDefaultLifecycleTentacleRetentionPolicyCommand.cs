using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    [Description("Modify the space default lifecycle tentacle retention policy.")]
    public class ModifyDefaultLifecycleTentacleRetentionPolicyCommand: ModifyDefaultRetentionPolicyCommand
    {
        public ModifyDefaultLifecycleTentacleRetentionPolicyCommand(string id, string spaceId) : base(id, spaceId)
        {
        }

        public override RetentionType RetentionType => RetentionType.LifecycleTentacle;
        
        [Description("Retention strategy for the default lifecycle tentacle retention policy. ['Count', 'Forever']")]
        [Writeable]
        [Required]
        public RetentionPeriodStrategy Strategy { get; set; }
        
        [Description("Quantity of release files/packages to keep for the default lifecycle tentacle retention policy.")]
        [Writeable]
        public int? QuantityToKeep { get; set; }
        
        [Description("Unit of measurement for the quantity of releases to keep. ['Days', 'Items']")]
        [Writeable]
        public LifecycleReleaseRetentionUnit Unit { get; set; }
    }
}
