﻿using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class RunbookStepsEditor : IResourceEditor<RunbookStepsResource, RunbookStepsEditor>
    {
        private readonly IRunbookStepsRepository repository;

        public RunbookStepsEditor(IRunbookStepsRepository repository)
        {
            this.repository = repository;
        }

        public RunbookStepsResource Instance { get; private set; }

        public RunbookStepsEditor Load(string id)
        {
            Instance = repository.Get(id);
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

        public RunbookStepsEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public RunbookStepsEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public RunbookStepsEditor Customize(Action<RunbookStepsResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public RunbookStepsEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}