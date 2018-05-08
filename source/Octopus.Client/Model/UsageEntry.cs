namespace Octopus.Client.Model
{
    public class UsageEntry<TEntry>
    {
        UsageEntry(bool isMissingScopedPermissions, string missingId)
        {
            IsMissingScopedPermissions = isMissingScopedPermissions;
            MissingId = missingId;
        }

        UsageEntry(TEntry entry)
        {
            Entry = entry;
        }

        public bool IsMissingScopedPermissions { get; }
        public string MissingId { get; }
        public TEntry Entry { get; }
    }
}