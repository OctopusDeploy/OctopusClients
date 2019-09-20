using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class OpsSnapshotResource : ReleaseBaseResource
    {
        [JsonConstructor]
        public OpsSnapshotResource()
        {
            SelectedPackages = new List<SelectedPackage>();
        }

        public OpsSnapshotResource(string projectId) : this()
        {
            ProjectId = projectId;
        }

        public string Name { get; set; }

        public string OpsStepsId { get; set; }

        public string FrozenOpsStepsId { get; set; }

        public string FrozenProjectVariableSetId { get; set; }
    }
}