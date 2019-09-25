using System.Collections.Generic;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Attributes;

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

        [Writeable]
        public string Name { get; set; }

        [WriteableOnCreate]
        public string OpsProcessId { get; set; }

        public string FrozenOpsStepsId { get; set; }

        public string FrozenProjectVariableSetId { get; set; }
    }
}