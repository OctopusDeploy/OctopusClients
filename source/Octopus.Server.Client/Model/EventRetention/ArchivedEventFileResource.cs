using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.EventRetention
{
    public class ArchivedEventFileResource : Resource, INamedResource
    {
        [Trim]
        public string Name { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public double FileBytes { get; set; }
    }
}
