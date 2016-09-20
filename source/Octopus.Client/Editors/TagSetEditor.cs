using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.DeploymentProcess;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors
{
    public class TagSetEditor : IResourceEditor<TagSetResource, TagSetEditor>
    {
        private readonly ITagSetRepository repository;

        public TagSetEditor(ITagSetRepository repository)
        {
            this.repository = repository;
        }

        public TagSetResource Instance { get; private set; }

        public async Task<TagSetEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TagSetResource
                {
                    Name = name,
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<TagSetEditor> CreateOrModify(string name, string description)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TagSetResource
                {
                    Name = name,
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Description = description;
                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public TagSetEditor ClearTags()
        {
            Instance.Tags.Clear();
            return this;
        }

        public TagSetEditor AddOrUpdateTag(
            string name,
            string description = null,
            string color = TagResource.StandardColor.DarkGrey)
        {
            Instance.AddOrUpdateTag(name, description, color);
            return this;
        }

        public TagSetEditor RemoveTag(string name)
        {
            Instance.RemoveTag(name);
            return this;
        }

        public TagSetEditor Customize(Action<TagSetResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<TagSetEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}