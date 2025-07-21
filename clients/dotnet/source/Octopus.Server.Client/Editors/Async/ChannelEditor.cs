using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ChannelEditor : IResourceEditor<ChannelResource, ChannelEditor>
    {
        private readonly IChannelRepository repository;

        public ChannelEditor(IChannelRepository repository)
        {
            this.repository = repository;
        }

        public ChannelResource Instance { get; private set; }

        public async Task<ChannelEditor> CreateOrModify(ProjectResource project, string name)
        {
            var existing = await repository.FindByName(project, name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = name
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            var existing = await repository.FindByName(project, name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
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

        public ChannelEditor ClearGitReferenceRules()
        {
            Instance.ClearGitReferenceRules();
            return this;
        }

        public ChannelEditor AddGitReferenceRule(string rule)
        {
            Instance.AddGitReferenceRule(rule);
            return this;
        }

        public ChannelEditor ClearGitResourceRules()
        {
            Instance.ClearGitResourceRules();
            return this;
        }

        public ChannelEditor AddGitResourceRule(ChannelGitResourceRuleResource rule)
        {
            Instance.AddGitResourceRule(rule);
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

        public async Task<ChannelEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}