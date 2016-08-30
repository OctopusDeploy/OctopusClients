namespace Octopus.Client.Model
{
    // TODO: [ObsoleteEx(TreatAsErrorFromVersion = "4.0", RemoveInVersion = "4.0", ReplacementTypeOrMember = "MachineModelHealthStatus")]
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