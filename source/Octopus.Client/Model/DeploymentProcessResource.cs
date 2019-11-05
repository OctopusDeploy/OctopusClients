using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class DeploymentProcessResource : Resource, IProcessResource, IHaveSpaceResource
    {
        public DeploymentProcessResource()
        {
            Steps = new List<DeploymentStepResource>();
        }

        public string ProjectId { get; set; }

        public IList<DeploymentStepResource> Steps { get; }

        [Required]
        public int Version { get; set; }

        public string LastSnapshotId { get; set; }

        public DeploymentStepResource FindStep(string name)
        {
            return Steps.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public DeploymentStepResource AddOrUpdateStep(string name)
        {
            var existing = FindStep(name);

            DeploymentStepResource step;
            if (existing == null)
            {
                step = new DeploymentStepResource
                {
                    Name = name
                };

                Steps.Add(step);
            }
            else
            {
                existing.Name = name;

                step = existing;
            }

            // Return the step so you can add actions
            return step;
        }

        public IProcessResource RemoveStep(string name)
        {
            Steps.Remove(FindStep(name));
            return this;
        }

        public IProcessResource ClearSteps()
        {
            Steps.Clear();
            return this;
        }

        public string SpaceId { get; set; }
    }
}