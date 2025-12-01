#nullable enable
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    public class SpaceDefaultRunbookRetentionPolicyResource(RunbookRetentionPolicyType strategy) : SpaceDefaultRetentionPolicyResource
    {
        public override RetentionType RetentionType => RetentionType.RunbookRetention;

        [Description("Retention strategy for the runbook retention policy. ['Count', 'Forever']")]
        [Writeable]
        [Required]
        public RunbookRetentionPolicyType Strategy { get; set; } = strategy;

        [Description("Quantity of units to keep for the default runbook retention policy.")]
        [Writeable]
        public int? QuantityToKeep { get; set; }

        [Description("Unit of measurement for the quantity of runbooks to keep. ['Days', 'Items']")]
        [Writeable]
        public RunbookRetentionUnit? Unit { get; set; }
    }
}
