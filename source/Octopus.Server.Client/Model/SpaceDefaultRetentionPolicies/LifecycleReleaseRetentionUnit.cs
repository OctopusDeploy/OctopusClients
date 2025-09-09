using Octopus.TinyTypes;

namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    public class LifecycleReleaseRetentionUnit : CaseInsensitiveStringTinyType
    {
        public static LifecycleReleaseRetentionUnit Items = new("Items");
        public static LifecycleReleaseRetentionUnit Days = new("Days");

        public LifecycleReleaseRetentionUnit(string value) : base(value)
        {
        }
    }
}
