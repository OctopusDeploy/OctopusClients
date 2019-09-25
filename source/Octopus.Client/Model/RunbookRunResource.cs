using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class OpsRunResource : DeploymentBaseResource
    {
        public OpsRunResource()
        {
            FormValues = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        [WriteableOnCreate]
        public string OpsSnapshotId { get; set; }

        public string FrozenOpsStepsId { get; set; }
        
        [Required(ErrorMessage = "Please specify the ops process to run.")]
        [WriteableOnCreate]
        public string OpsProcessId { get; set; }
    }
}