using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

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

        public TagSetEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new TagSetResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Name = name;
                Instance = repository.Modify(existing);
            }

            return this;
        }

        public TagSetEditor CreateOrModify(string name, string description)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new TagSetResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Description = description;
                Instance = repository.Modify(existing);
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

        public TagSetEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}