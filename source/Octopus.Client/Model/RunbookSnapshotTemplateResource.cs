using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class RunbookSnapshotTemplateResource : ReleaseTemplateBaseResource
    {
        public string RunbookStepsId { get; set; }
        public string NextNameIncrement { get; set; }
    }
}