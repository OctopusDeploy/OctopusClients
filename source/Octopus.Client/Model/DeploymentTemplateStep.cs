using System;

namespace Octopus.Client.Model
{
    public class DeploymentTemplateStep
    {
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionNumber { get; set; }
        public string[] Roles { get; set; }

        /// <summary>
        /// Obsoleted as Server 3.4
        /// </summary>
        public string[] MachineNames { get; set; }

        public MachineDeploymentPreview[] Machines { get; set; }
        public bool CanBeSkipped { get; set; }
        public bool HasNoApplicableMachines { get; set; }
        public ReferenceDataItem[] UnavailableMachines { get; set; }
        public ReferenceDataItem[] ExcludedMachines { get; set; }
    }
}