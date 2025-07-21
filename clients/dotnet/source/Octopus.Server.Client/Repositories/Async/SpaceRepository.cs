﻿using System.IO;
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
        Task SetLogo(SpaceResource space, string fileName, Stream contents);
        Task<SpaceSearchResult[]> Search(string spaceId, string keyword);
    }

    class SpaceRepository : BasicRepository<SpaceResource>, ISpaceRepository
    {
        public SpaceRepository(IOctopusAsyncRepository repository) : base(repository, "Spaces")
        {
        }

        public Task SetLogo(SpaceResource space, string fileName, Stream contents)
        {
            return Client.Post(space.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public async Task<SpaceSearchResult[]> Search(string spaceId, string keyword)
        {
            return await Client.Get<SpaceSearchResult[]>(await Repository.Link("SpaceSearch").ConfigureAwait(false), new { id = spaceId, keyword }).ConfigureAwait(false);
        }
    }
}
