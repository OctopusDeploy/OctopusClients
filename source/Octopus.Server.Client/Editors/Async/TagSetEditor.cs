using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class TagSetEditor : IResourceEditor<TagSetResource, TagSetEditor>
    {
        private readonly ITagSetRepository repository;

        public TagSetEditor(ITagSetRepository repository)
        {
            this.repository = repository;
        }

        public TagSetResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TagSetEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<TagSetEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TagSetResource
                {
                    Name = name,
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
        public Task<TagSetEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<TagSetEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new TagSetResource
                {
                    Name = name,
                    Description = description,
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TagSetEditor> Save()
            => Save(CancellationToken.None);

        public async Task<TagSetEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
