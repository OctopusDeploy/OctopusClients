using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ISpaceRepository :
        ICreate<SpaceResource>,
        IModify<SpaceResource>,
        IDelete<SpaceResource>,
        IFindByName<SpaceResource>,
        IGet<SpaceResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SetLogo(SpaceResource space, string fileName, Stream contents);
        Task SetLogo(SpaceResource space, string fileName, Stream contents, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<SpaceSearchResult[]> Search(string spaceId, string keyword);
        Task<SpaceSearchResult[]> Search(string spaceId, string keyword, CancellationToken cancellationToken);
    }

    class SpaceRepository : BasicRepository<SpaceResource>, ISpaceRepository
    {
        public SpaceRepository(IOctopusAsyncRepository repository) : base(repository, "Spaces")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task SetLogo(SpaceResource space, string fileName, Stream contents)
            => SetLogo(space, fileName, contents, CancellationToken.None);

        public Task SetLogo(SpaceResource space, string fileName, Stream contents, CancellationToken cancellationToken)
        {
            return Client.Post(space.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SpaceSearchResult[]> Search(string spaceId, string keyword)
            => Search(spaceId, keyword, CancellationToken.None);

        public async Task<SpaceSearchResult[]> Search(string spaceId, string keyword, CancellationToken cancellationToken)
        {
            return await Client.Get<SpaceSearchResult[]>(await Repository.Link("SpaceSearch").ConfigureAwait(false), new { id = spaceId, keyword }, cancellationToken).ConfigureAwait(false);
        }
    }
}
