using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    [Description("Modify space default retention policies.")]
    public abstract class ModifyDefaultRetentionPolicyCommand
    {
        protected ModifyDefaultRetentionPolicyCommand(string id, string spaceId)
        {
            Id = id;
            SpaceId = spaceId;
        }
        [Description("Id of the space default retention policy.")]
        [Writeable]
        [Required]
        public string Id { get; set; }
        
        [Description("SpaceId of the space default retention policy.")]
        [Writeable]
        [Required]
        public string SpaceId { get; set; }
        
        [Description("Type of the space default retention policy. ['MachinePackageCache', 'LifecycleRelease', 'LifecycleTentacle', 'RunbookRetention']")]
        public abstract RetentionType RetentionType { get; }
    }
}
