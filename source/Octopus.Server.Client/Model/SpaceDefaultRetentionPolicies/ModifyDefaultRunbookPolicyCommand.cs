using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    [Description("Modify the space default runbook retention policy.")]
    public class ModifyDefaultRunbookRetentionPolicyCommand : ModifyDefaultRetentionPolicyCommand
    {
        public ModifyDefaultRunbookRetentionPolicyCommand(string id, string spaceId) : base(id, spaceId)
        {
        }

        public override RetentionType RetentionType => RetentionType.RunbookRetention;

        [Description("Retention strategy for the default runbook retention policy. ['Count', 'Forever']")]
        [Writeable]
        [Required]
        public RunbookRetentionPolicyType Strategy { get; set; }

        [Description("Quantity of runbooks to keep for the default runbook retention policy.")]
        [Writeable]
        public int? QuantityToKeep { get; set; }

        [Description("Unit of measurement for the quantity of runbooks to keep. ['Days', 'Items']")]
        [Writeable]
        public RunbookRetentionUnit Unit { get; set; }
    }
}
