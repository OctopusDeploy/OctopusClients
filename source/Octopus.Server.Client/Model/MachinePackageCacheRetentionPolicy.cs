namespace Octopus.Client.Model;

public enum MachinePackageCacheRetentionStrategy
{
    Default,
    Quantities,
}

public enum MachinePackageCacheRetentionUnit
{
    Items
}

public class MachinePackageCacheRetentionPolicy
{
    public MachinePackageCacheRetentionStrategy Strategy { get; set; } = MachinePackageCacheRetentionStrategy.Default;
    public int? QuantityOfPackagesToKeep { get; set; }
    public MachinePackageCacheRetentionUnit? PackageUnit { get; set; }
    public int? QuantityOfVersionsToKeep { get; set; }
    public MachinePackageCacheRetentionUnit? VersionUnit { get; set; }
}