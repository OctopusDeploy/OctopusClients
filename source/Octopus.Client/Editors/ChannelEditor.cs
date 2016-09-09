using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class ChannelEditor : IResourceEditor<ChannelResource, ChannelEditor>
    {
        private readonly IChannelRepository repository;

        public ChannelEditor(IChannelRepository repository)
        {
            this.repository = repository;
        }

        public ChannelResource Instance { get; private set; }

        public ChannelEditor CreateOrModify(ProjectResource project, string name)
        {
            var existing = repository.FindByName(project, name);

            if (existing == null)
            {
                Instance = repository.Create(new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = name
                });
            }
            else
            {
                existing.Name = name;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public ChannelEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            var existing = repository.FindByName(project, name);

            if (existing == null)
            {
                Instance = repository.Create(new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public ChannelEditor SetAsDefaultChannel()
        {
            Instance.SetAsDefaultChannel();
            return this;
        }

        public ChannelEditor UsingLifecycle(LifecycleResource lifecycle)
        {
            Instance.UsingLifecycle(lifecycle);
            return this;
        }

        public ChannelEditor ClearRules()
        {
            Instance.ClearRules();
            return this;
        }

        public ChannelEditor AddRule(ChannelVersionRuleResource rule)
        {
            Instance.AddRule(rule);
            return this;
        }

        public ChannelEditor AddCommonRuleForAllActions(string versionRange, string tagRegex, DeploymentProcessResource process)
        {
            Instance.AddCommonRuleForAllActions(versionRange, tagRegex, process);
            return this;
        }

        public ChannelEditor AddRule(string versionRange, string tagRegex, params DeploymentActionResource[] actions)
        {
            Instance.AddRule(versionRange, tagRegex, actions);
            return this;
        }

        public ChannelEditor ClearTenantTags()
        {
            Instance.ClearTenantTags();
            return this;
        }

        public ChannelEditor AddOrUpdateTenantTags(params TagResource[] tags)
        {
            Instance.AddOrUpdateTenantTags(tags);
            return this;
        }

        public ChannelEditor Customize(Action<ChannelResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public ChannelEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}