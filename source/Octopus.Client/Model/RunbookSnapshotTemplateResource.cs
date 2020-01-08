using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class RunbookSnapshotTemplateResource : ReleaseTemplateBaseResource
    {
        public string RunbookProcessId { get; set; }
        public string NextNameIncrement { get; set; }
    }
}