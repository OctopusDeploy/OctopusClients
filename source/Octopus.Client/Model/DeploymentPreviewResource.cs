using System;
using System.Collections.Generic;
using Octopus.Client.Model.Forms;
using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model
{
    public class DeploymentPreviewResource : Resource
    {
        public List<DeploymentTemplateStep> StepsToExecute { get; set; }
        public Form Form { get; set; }
        public bool UseGuidedFailureModeByDefault { get; set; }

        public string ReleaseNotes { get; set; }
        public List<WorkItem> WorkItems { get; set; }
    }
}