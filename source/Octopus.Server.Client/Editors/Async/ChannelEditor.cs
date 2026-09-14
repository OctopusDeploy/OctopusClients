using System;
using System.Threading;
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name)
            => CreateOrModify(project, name, CancellationToken.None);

        public async Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(project, name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = name
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description)
            => CreateOrModify(project, name, description, CancellationToken.None);

        public async Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(project, name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ChannelResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> Save()
            => Save(CancellationToken.None);

        public async Task<ChannelEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
