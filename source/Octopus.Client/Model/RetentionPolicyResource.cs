using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RetentionPolicyResource : Resource, INamedResource
    {
        [Writeable]
        public string Name { get; set; }

        public bool IsReadOnly { get; set; }

        [Writeable]
        public RetentionPeriod UndeployedReleases { get; set; }

        [Writeable]
        public RetentionPeriod DeployedReleases { get; set; }

        [Writeable]
        public RetentionPeriod TentacleDeployments { get; set; }
    }
}