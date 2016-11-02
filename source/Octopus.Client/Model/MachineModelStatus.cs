using System;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Obsoleted as Server 3.4
    /// </summary>
    public enum MachineModelStatus
    {
        Online,
        Offline,
        Unknown,
        NeedsUpgrade,
        CalamariNeedsUpgrade,
        Disabled
    }
}