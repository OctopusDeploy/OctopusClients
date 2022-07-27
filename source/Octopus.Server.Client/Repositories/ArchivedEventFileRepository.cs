using System.IO;
using Octopus.Client.Model;
using Octopus.Client.Model.EventRetention;

namespace Octopus.Client.Repositories
{
    public interface IArchivedEventFileRepository : IDelete<ArchivedEventFileResource>
    {
        Stream GetContent(ArchivedEventFileResource archiveEventFile);
        ResourceCollection<ArchivedEventFileResource> List(int skip = 0, int? take = null);
    }

    class ArchivedEventFileRepository : BasicRepository<ArchivedEventFileResource>, IArchivedEventFileRepository
    {
        public ArchivedEventFileRepository(IOctopusRepository repository) : base(repository, "ArchivedEventFiles")
        {
            MinimumCompatibleVersion("2022.3.6072");
        }

        public Stream GetContent(ArchivedEventFileResource archiveEventFile)
        {
            ThrowIfServerVersionIsNotCompatible();

            return Client.GetContent(archiveEventFile.Link("Self"));
        }

        public ResourceCollection<ArchivedEventFileResource> List(int skip = 0, int? take = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            return Client.List<ArchivedEventFileResource>(
                Repository.Link("events/archives"),
                new
                {
                    skip,
                    take
                });
        }
    }
}
