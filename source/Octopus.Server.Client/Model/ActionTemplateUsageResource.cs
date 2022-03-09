using System;

namespace Octopus.Client.Model
{
    public class ActionTemplateUsageResource : Resource
    {
        public string ActionTemplateId { get; set; }
        public string RunbookId { get; set; }
        public ProcessType ProcessType { get; set; }
        public string ProcessId { get; set; }
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string RunbookName { get; set; }
        public string StepId { get; set; }
        public string StepName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectSlug { get; set; }
        public string Version { get; set; }

        [Obsolete("Use " + nameof(ProcessId) + " with " + nameof(ProcessType) + " instead")]
        public string DeploymentProcessId { get; set; }
        //used for VCS only
        public string Branch { get; set; }
        public string Release { get; set; }
    }
}