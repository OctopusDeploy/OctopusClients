namespace Octopus.Client.Model
{
    public class ProjectedTeamReferenceDataItem : ReferenceDataItem
    {
        public bool IsDirectlyAssigned { get; set; }
        public string[] ExternalGroupNames { get; set; }
        public string SpaceId { get; set; }

        public ProjectedTeamReferenceDataItem(string id, string name, string spaceId, bool isDirectlyAssigned, string[] externalGroupNames) : base(id, name)
        {
            ExternalGroupNames = externalGroupNames;
            IsDirectlyAssigned = isDirectlyAssigned;
            SpaceId = spaceId;
        }
    }
}