using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility.Attributes;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class LifecycleResource : Resource, INamedResource, IHaveSpaceResource
    {
        public LifecycleResource()
        {
            Phases = new List<PhaseResource>();
            ReleaseRetentionPolicy = RetentionPeriod.KeepForever();
            TentacleRetentionPolicy = RetentionPeriod.KeepForever();
        }

        [Writeable]
        [JsonProperty(Order = 2)]
        public string Name { get; set; }

        [Writeable]
        [JsonProperty(Order = 20)]
        public string Description { get; set; }

        [Writeable]
        [JsonProperty(Order = 5, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public RetentionPeriod ReleaseRetentionPolicy { get; set; }

        [Writeable]
        [JsonProperty(Order = 6, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public RetentionPeriod TentacleRetentionPolicy { get; set; }

        public IList<PhaseResource> Phases { get; private set; }

        public LifecycleResource WithReleaseRetentionPolicy(RetentionPeriod period)
        {
            ReleaseRetentionPolicy = period;
            return this;
        }

        public LifecycleResource WithTentacleRetentionPolicy(RetentionPeriod period)
        {
            TentacleRetentionPolicy = period;
            return this;
        }

        public LifecycleResource Clear()
        {
            Phases.Clear();
            return this;
        }

        public PhaseResource FindPhase(string name)
        {
            return Phases.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public PhaseResource AddOrUpdatePhase(string name)
        {
            var phase = FindPhase(name);
            if (phase == null)
            {
                phase = new PhaseResource
                {
                    Name = name,
                };
                Phases.Add(phase);
            }
            else
            {
                phase.Name = name;
            }

            return phase;
        }

        public LifecycleResource RemovePhase(string name)
        {
            var existing = FindPhase(name);
            if (existing != null) Phases.Remove(existing);
            return this;
        }

        public string SpaceId { get; set; }
    }
}