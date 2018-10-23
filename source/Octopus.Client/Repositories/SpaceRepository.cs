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
    }
}
