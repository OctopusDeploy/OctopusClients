using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ProgressionResource : Resource
    {
        public List<ReferenceDataItem> Environments { get; set; }
        public Dictionary<string, IEnumerable<ReferenceDataItem>> ChannelEnvironments { get; set; } 
        public List<ReleaseProgressionResource> Releases { get; set; }
    }
}