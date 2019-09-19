using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class OpsSnapshotTemplateResource : ReleaseTemplateBaseResource
    {
        public string OpsStepsId { get; set; }
        public string NextNameIncrement { get; set; }
    }
}