using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITagSetRepository : ICreate<TagSetResource>, IModify<TagSetResource>, IGet<TagSetResource>, IDelete<TagSetResource>, IFindByName<TagSetResource>, IGetAll<TagSetResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Sort(string[] tagSetIdsInOrder);
        Task Sort(string[] tagSetIdsInOrder, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TagSetEditor> CreateOrModify(string name);
        Task<TagSetEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TagSetEditor> CreateOrModify(string name, string description);
        Task<TagSetEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
    }

    class TagSetRepository : BasicRepository<TagSetResource>, ITagSetRepository
    {
        public TagSetRepository(IOctopusAsyncRepository repository) : base(repository, "TagSets")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Sort(string[] tagSetIdsInOrder)
            => Sort(tagSetIdsInOrder, CancellationToken.None);

        public async Task Sort(string[] tagSetIdsInOrder, CancellationToken cancellationToken)
        {
            await Client.Put(await Repository.Link("TagSetSortOrder").ConfigureAwait(false), tagSetIdsInOrder, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TagSetEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public Task<TagSetEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new TagSetEditor(this).CreateOrModify(name);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TagSetEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public Task<TagSetEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new TagSetEditor(this).CreateOrModify(name, description);
        }
    }
}
