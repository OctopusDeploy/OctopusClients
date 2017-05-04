using System;

namespace Octopus.Client.Model
{
    public class PhaseResource
    {
        public PhaseResource()
        {
            AutomaticDeploymentTargets = new ReferenceCollection();
            OptionalDeploymentTargets = new ReferenceCollection();
        }

        public string Id { get; set; }

        [Trim]
        public string Name { get; set; }

        public ReferenceCollection AutomaticDeploymentTargets { get; set; }
        public ReferenceCollection OptionalDeploymentTargets { get; set; }
        public int MinimumEnvironmentsBeforePromotion { get; set; }
        public bool IsOptionalPhase { get; set; }
        public RetentionPeriod ReleaseRetentionPolicy { get; set; }
        public RetentionPeriod TentacleRetentionPolicy { get; set; }

        public PhaseResource Clear()
        {
            AutomaticDeploymentTargets.Clear();
            OptionalDeploymentTargets.Clear();
            MinimumEnvironmentsBeforePromotion = 0;
            ReleaseRetentionPolicy = null;
            TentacleRetentionPolicy = null;

            return this;
        }

        public PhaseResource WithAutomaticDeploymentTargets(params EnvironmentResource[] environments)
        {
            foreach (var environment in environments)
            {
                AutomaticDeploymentTargets.Add(environment.Id);
            }
            return this;
        }

        public PhaseResource WithOptionalDeploymentTargets(params EnvironmentResource[] environments)
        {
            foreach (var environment in environments)
            {
                OptionalDeploymentTargets.Add(environment.Id);
            }
            return this;
        }

        public PhaseResource WithMinimumEnvironmentsBeforePromotion(int minimumEnvironmentsBeforePromotion)
        {
            MinimumEnvironmentsBeforePromotion = minimumEnvironmentsBeforePromotion;
            return this;
        }

        public PhaseResource WithReleaseRetentionPolicy(RetentionPeriod period)
        {
            ReleaseRetentionPolicy = period;
            return this;
        }

        public PhaseResource WithTentacleRetentionPolicy(RetentionPeriod period)
        {
            TentacleRetentionPolicy = period;
            return this;
        }
    }
}