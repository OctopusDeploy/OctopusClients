using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model
{
    public class ReleaseProgressionResource
    {
        public ReleaseResource Release { get; set; }
        public ChannelResource Channel { get; set; }
        public Dictionary<string, IList<DashboardItemResource>> Deployments { get; set; }
        public ReferenceCollection NextDeployments { get; set; }
        public bool HasUnresolvedDefect { get; set; }
        public RetentionPeriod ReleaseRetentionPeriod { get; set; }
        public RetentionPeriod TentacleRetentionPeriod { get; set; }
    }
}