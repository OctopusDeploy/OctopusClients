using System;
using System.Collections.Generic;
using Octopus.Client.Model.Forms;

namespace Octopus.Client.Model
{
    public class DeploymentPreviewResource : Resource
    {
        public List<DeploymentTemplateStep> StepsToExecute { get; set; }
        public Form Form { get; set; }
        public bool UseGuidedFailureModeByDefault { get; set; }

        public List<ReleaseChanges> Changes { get; set; }
        public string ChangesMarkdown { get; set; }
    }
}