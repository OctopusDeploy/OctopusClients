namespace Octopus.Client.Model;

public enum MachinePackageCacheRetentionUnit
{
    Items
}

public class MachinePackageCacheRetentionPolicy
{
    public int QuantityOfPackagesToKeep { get; set; }
    public MachinePackageCacheRetentionUnit PackageUnit { get; set; } = MachinePackageCacheRetentionUnit.Items;
    public int QuantityOfVersionsToKeep { get; set; }
    public MachinePackageCacheRetentionUnit VersionUnit { get; set; } = MachinePackageCacheRetentionUnit.Items;
}