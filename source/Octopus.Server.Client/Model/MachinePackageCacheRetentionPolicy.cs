using System;

namespace Octopus.Client.Model;

public class MachinePackageCacheRetentionPolicy
{
    public int QuantityToKeep { get; set; }
    public MachinePackageCacheRetentionUnit Unit { get; set; }
}
