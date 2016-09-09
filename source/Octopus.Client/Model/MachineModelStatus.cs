using System;

namespace Octopus.Client.Model
{
    [Obsolete] // TODO: [ObsoleteEx(TreatAsErrorFromVersion = "4.0", RemoveInVersion = "4.0", ReplacementTypeOrMember = "MachineModelHealthStatus")]
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