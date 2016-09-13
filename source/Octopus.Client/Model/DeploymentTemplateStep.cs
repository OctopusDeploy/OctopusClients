using System;

namespace Octopus.Client.Model
{
    public class DeploymentTemplateStep
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionNumber { get; set; }
        public string[] Roles { get; set; }

        [Obsolete] // TODO: [ObsoleteEx(TreatAsErrorFromVersion = "4.0", RemoveInVersion = "4.0", ReplacementTypeOrMember = "Machines")]
        public string[] MachineNames { get; set; }

        public MachineDeploymentPreview[] Machines { get; set; }
        public bool CanBeSkipped { get; set; }
        public bool HasNoApplicableMachines { get; set; }
        public ReferenceDataItem[] UnavailableMachines { get; set; }
        public ReferenceDataItem[] ExcludedMachines { get; set; }
    }
}