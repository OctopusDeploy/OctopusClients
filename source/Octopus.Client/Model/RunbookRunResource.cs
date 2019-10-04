using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunbookRunResource : DeploymentBaseResource
    {
        public RunbookRunResource()
        {
            FormValues = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        [WriteableOnCreate]
        public string RunbookSnapshotId { get; set; }

        public string FrozenRunbookProcessId { get; set; }
        
        [Required(ErrorMessage = "Please specify the Runbook to run.")]
        [WriteableOnCreate]
        public string RunbookId { get; set; }
    }
}