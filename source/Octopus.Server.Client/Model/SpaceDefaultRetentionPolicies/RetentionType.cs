using Octopus.TinyTypes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    public class RetentionType(string value) : CaseInsensitiveStringTinyType(value)
    {
        public static readonly RetentionType MachinePackageCache = new(nameof(MachinePackageCache));
        public static readonly RetentionType LifecycleRelease = new(nameof(LifecycleRelease));
        public static readonly RetentionType LifecycleTentacle = new(nameof(LifecycleTentacle));
        public static readonly RetentionType RunbookRetention = new(nameof(RunbookRetention));
    }

    public class ReleaseRetentionPolicyType(string value) : CaseInsensitiveStringTinyType(value)
    {
        public static readonly ReleaseRetentionPolicyType Count = new(nameof(Count));
        public static readonly ReleaseRetentionPolicyType Forever = new(nameof(Forever));
    }
    
    public class TentacleRetentionPolicyType(string value) : CaseInsensitiveStringTinyType(value)
    {
        public static readonly TentacleRetentionPolicyType Count = new(nameof(Count));
        public static readonly TentacleRetentionPolicyType Forever = new(nameof(Forever));
    }
}
