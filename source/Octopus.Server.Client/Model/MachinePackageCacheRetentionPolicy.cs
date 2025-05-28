namespace Octopus.Client.Model;

public enum MachinePackageCacheRetentionStrategy
{
    FreeSpace,
    Quantities,
}

public enum MachinePackageCacheRetentionUnit
{
    Items
}

public class MachinePackageCacheRetentionPolicy
{
    public MachinePackageCacheRetentionStrategy Strategy { get; set; } = MachinePackageCacheRetentionStrategy.FreeSpace;
    public int? QuantityOfPackagesToKeep { get; set; }
    public MachinePackageCacheRetentionUnit? PackageUnit { get; set; }
    public int? QuantityOfVersionsToKeep { get; set; }
    public MachinePackageCacheRetentionUnit? VersionUnit { get; set; }
}