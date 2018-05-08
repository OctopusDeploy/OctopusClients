namespace Octopus.Client.Model
{
    public class UsageEntry<TEntry>
    {
        public bool IsMissingScopedPermissions { get; set; }
        public string MissingId { get; set;}
        public TEntry Entry { get; set;}
    }
}