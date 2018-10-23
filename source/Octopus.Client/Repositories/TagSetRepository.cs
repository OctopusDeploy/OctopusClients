using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITagSetRepository : ICreate<TagSetResource>, IModify<TagSetResource>, IGet<TagSetResource>, IDelete<TagSetResource>, IFindByName<TagSetResource>, IGetAll<TagSetResource>
    {
        void Sort(string[] tagSetIdsInOrder);
        TagSetEditor CreateOrModify(string name);
        TagSetEditor CreateOrModify(string name, string description);
    }
    
    class TagSetRepository : BasicRepository<TagSetResource>, ITagSetRepository
    {
        public TagSetRepository(IOctopusRepository repository) : base(repository, "TagSets")
        {
        }

        public void Sort(string[] tagSetIdsInOrder)
        {
            Client.Put(Repository.Link("TagSetSortOrder"), tagSetIdsInOrder);
        }

        public TagSetEditor CreateOrModify(string name)
        {
            return new TagSetEditor(this).CreateOrModify(name);
        }

        public TagSetEditor CreateOrModify(string name, string description)
        {
            return new TagSetEditor(this).CreateOrModify(name, description);
        }
    }
}