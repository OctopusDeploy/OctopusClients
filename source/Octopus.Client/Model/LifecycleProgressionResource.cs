using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class LifecycleProgressionResource : Resource
    {
        public LifecycleProgressionResource()
        {
            Phases = new List<PhaseProgressionResource>();
        }

        public List<PhaseProgressionResource> Phases { get; set; }
        public ReferenceCollection NextDeployments { get; set; }
        public int NextDeploymentsMinimumRequired { get; set; }
    }
}