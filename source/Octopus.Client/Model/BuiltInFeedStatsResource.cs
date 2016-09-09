using System;

namespace Octopus.Client.Model
{
    public class BuiltInFeedStatsResource : Resource
    {
        public int TotalPackages { get; set; }
        public string SynchronizationStatus { get; set; }
        public string IndexingStatus { get; set; }
    }
}