namespace Octopus.Client.Model
{
    public class ActionUpdateRemovedPackageUsage
    {
        public ActionUpdateRemovedPackageUsage(string packageReference, ActionUpdatePackageUsedBy usedBy, string usedById, string usedByName)
        {
            PackageReference = packageReference;
            UsedBy = usedBy;
            UsedById = usedById;
            UsedByName = usedByName;
        }

        public string PackageReference { get; }
        public ActionUpdatePackageUsedBy UsedBy { get; }
        public string UsedById { get; }
        public string UsedByName { get; }
    }
}