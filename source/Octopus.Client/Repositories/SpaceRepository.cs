using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ISpaceRepository :
        ICreate<SpaceResource>,
        IModify<SpaceResource>,
        IDelete<SpaceResource>,
        IFindByName<SpaceResource>,
        IGet<SpaceResource>
    {
        void SetLogo(SpaceResource space, string fileName, Stream contents);
        SpaceSearchResult[] Search(SpaceResource space, string keyword);
        SpaceSearchResult[] Search(string spaceId, string keyword);
    }

    class SpaceRepository : BasicRepository<SpaceResource>, ISpaceRepository
    {
        public SpaceRepository(IOctopusRepository repository) : base(repository, "Spaces")
        {
        }

        public void SetLogo(SpaceResource space, string fileName, Stream contents)
        {
            Client.Post(space.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }

        public SpaceSearchResult[] Search(SpaceResource space, string keyword)
        {
            return Client.Get<SpaceSearchResult[]>(space.Link("Search"), new { Keyword = keyword });
        }

        public SpaceSearchResult[] Search(string spaceId, string keyword)
        {
            return Client.Get<SpaceSearchResult[]>(Repository.Link("SpaceSearch"), new { id = spaceId, Keyword = keyword });
        }
    }
}
