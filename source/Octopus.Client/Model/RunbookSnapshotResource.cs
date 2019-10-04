using System.Collections.Generic;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunbookSnapshotResource : ReleaseBaseResource
    {
        [JsonConstructor]
        public RunbookSnapshotResource()
        {
            SelectedPackages = new List<SelectedPackage>();
        }

        public RunbookSnapshotResource(string projectId) : this()
        {
            ProjectId = projectId;
        }

        [Writeable]
        public string Name { get; set; }

        [WriteableOnCreate]
        public string RunbookId { get; set; }

        public string FrozenRunbookProcessId { get; set; }

        public string FrozenProjectVariableSetId { get; set; }
    }
}