namespace Octopus.Client.Model.SpaceDefaultRetentionPolicies
{
    /// <summary>
    /// Represents a space default retention policy.
    /// </summary>
    public abstract class SpaceDefaultRetentionPolicyResource
    {
        public string SpaceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public abstract RetentionType RetentionType { get; }
    }
}
