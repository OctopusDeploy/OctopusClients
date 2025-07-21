﻿using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class DeploymentProcessEditor : IResourceEditor<DeploymentProcessResource, DeploymentProcessEditor>
    {
        private readonly IDeploymentProcessRepository repository;

        public DeploymentProcessEditor(IDeploymentProcessRepository repository)
        {
            this.repository = repository;
        }

        public DeploymentProcessResource Instance { get; private set; }

        public async Task<DeploymentProcessEditor> Load(string id)
        {
            Instance = await repository.Get(id).ConfigureAwait(false);
            return this;
        }

        public DeploymentStepResource FindStep(string name)
        {
            return Instance.FindStep(name);
        }

        public DeploymentStepResource AddOrUpdateStep(string name)
        {
            return Instance.AddOrUpdateStep(name);
        }

        public DeploymentProcessEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public DeploymentProcessEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public DeploymentProcessEditor Customize(Action<DeploymentProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<DeploymentProcessEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}