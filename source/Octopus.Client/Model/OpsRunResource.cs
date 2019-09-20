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

        public string OpsSnapshotId { get; set; }

        [WriteableOnCreate]
        public string FrozenOpsStepsId { get; set; }

        [Required(ErrorMessage = "Please specify the ops steps collection to run.")]
        [WriteableOnCreate]
        public string OpsStepsId { get; set; }
    }
}