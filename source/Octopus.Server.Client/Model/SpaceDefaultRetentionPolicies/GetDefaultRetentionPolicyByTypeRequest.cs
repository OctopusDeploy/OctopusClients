using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    [Description("Get the configured default retention policies for the given retention type")]
    public class GetDefaultRetentionPolicyByTypeRequest
    {
        public GetDefaultRetentionPolicyByTypeRequest() {}
        
        public GetDefaultRetentionPolicyByTypeRequest(string spaceId, RetentionType retentionType)
        {
            SpaceId = spaceId;
            RetentionType = retentionType;
        }
        [Description("SpaceId of the space default retention policy.")]
        [Required]
        [Writeable]
        public string SpaceId { get; set; }
        
        [Description("Type of the space default retention policy. ['MachinePackageCache', 'LifecycleRelease', 'LifecycleTentacle', 'RunbookRetention']")]
        [Required]
        [Writeable]
        public RetentionType RetentionType { get; set; }
    }
}
