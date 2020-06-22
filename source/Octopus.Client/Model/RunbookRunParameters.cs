using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model
{
    public class RunbookRunParameters
    {
        /// <summary>
        /// Parameter class for marshalling params between OctopusCLI and Octopus Server
        /// This class is used to facilitate backwards compatibility while extending /runbooks/{id}/run"
        /// </summary>
        public RunbookRunParameters()
        {
            FormValues = new Dictionary<string, string>();
            SpecificMachineIds = new string[0];
            ExcludedMachineIds = new string[0];
            EnvironmentIds = new string[0];
            TenantTagNames = new string[0];
            TenantIds = new string[0];
            SkipActions = new string[0];
        }

        public bool UseDefaultSnapshot { get; set; } = true;

        public string RunbookId { get; set; }
        public string ProjectId { get; set; }
        public string RunbookSnapshotNameOrId { get; set; }
        public string EnvironmentId { get; set; }
        public string[] EnvironmentIds { get; set; }
        public bool ForcePackageDownload { get; set; }
        public bool? UseGuidedFailure { get; set; }
        public string[] SpecificMachineIds { get; set; }
        public string[] ExcludedMachineIds { get; set; }
        public string TenantId { get; set; }
        public string[] TenantIds { get; set; }
        public string[] TenantTagNames { get; set; }
        public string[] SkipActions { get; set; }
        public DateTimeOffset? QueueTime { get; set; }
        public DateTimeOffset? QueueTimeExpiry { get; set; }
        public Dictionary<string, string> FormValues { get; set; }

        public static RunbookRunParameters MapFrom(RunbookRunResource runbookRun)
        {
            return new RunbookRunParameters
            {
                UseDefaultSnapshot = true,
                RunbookId = runbookRun.RunbookId,
                ProjectId = runbookRun.ProjectId,
                EnvironmentId = runbookRun.EnvironmentId,
                ForcePackageDownload = runbookRun.ForcePackageDownload,
                UseGuidedFailure = runbookRun.UseGuidedFailure,
                SpecificMachineIds = runbookRun.SpecificMachineIds.ToArray(),
                ExcludedMachineIds = runbookRun.ExcludedMachineIds.ToArray(),
                TenantId = runbookRun.TenantId,
                SkipActions = runbookRun.SkipActions.ToArray(),
                QueueTime = runbookRun.QueueTime,
                QueueTimeExpiry = runbookRun.QueueTimeExpiry,
                FormValues = runbookRun.FormValues
            };
        }
    }
}