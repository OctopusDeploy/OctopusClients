using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class PhaseProgressionResource
    {
        public PhaseProgressionResource()
        {
            Deployments = new List<PhaseDeploymentResource>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Blocked { get; set; }
        public PhaseProgress Progress { get; set; }
        public List<PhaseDeploymentResource> Deployments { get; set; }
        public ReferenceCollection AutomaticDeploymentTargets { get; set; }
        public ReferenceCollection OptionalDeploymentTargets { get; set; }
        public int MinimumEnvironmentsBeforePromotion { get; set; }
        public bool IsOptionalPhase { get; set; }
        public bool IsPriorityPhase { get; set; }
    }
}
