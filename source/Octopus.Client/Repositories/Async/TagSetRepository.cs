using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITagSetRepository : ICreate<TagSetResource>, IModify<TagSetResource>, IGet<TagSetResource>, IDelete<TagSetResource>, IFindByName<TagSetResource>, IGetAll<TagSetResource>
    {
        Task Sort(string[] tagSetIdsInOrder);
        Task<TagSetEditor> CreateOrModify(string name);
        Task<TagSetEditor> CreateOrModify(string name, string description);
    }

    class TagSetRepository : BasicRepository<TagSetResource>, ITagSetRepository
    {
        public TagSetRepository(IOctopusAsyncRepository repository) : base(repository, "TagSets")
        {
        }

        public Task Sort(string[] tagSetIdsInOrder)
        {
            return Client.Put(Repository.Link("TagSetSortOrder"), tagSetIdsInOrder);
        }

        public Task<TagSetEditor> CreateOrModify(string name)
        {
            return new TagSetEditor(this).CreateOrModify(name);
        }

        public Task<TagSetEditor> CreateOrModify(string name, string description)
        {
            return new TagSetEditor(this).CreateOrModify(name, description);
        }
    }
}
