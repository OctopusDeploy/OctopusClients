using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Model.EventRetention;

namespace Octopus.Client.Repositories.Async
{
    public interface IArchivedEventFileRepository : 
        IDelete<ArchivedEventFileResource>
    {
        Task<Stream> GetContent(ArchivedEventFileResource archiveEventFile);
        Task<ResourceCollection<ArchivedEventFileResource>> List(int skip = 0, int? take = null);
    }

    class ArchivedEventFileRepository : BasicRepository<ArchivedEventFileResource>, IArchivedEventFileRepository
    {
        public ArchivedEventFileRepository(IOctopusAsyncRepository repository) : base(repository, "ArchivedEventFiles")
        {
            MinimumCompatibleVersion("2022.3.6072");
        }

        public async Task<Stream> GetContent(ArchivedEventFileResource archiveEventFile)
        {
            await ThrowIfServerVersionIsNotCompatible();

            return await Client.GetContent(archiveEventFile.Link("Self"));
        }

        public async Task<ResourceCollection<ArchivedEventFileResource>> List(int skip = 0, int? take = null)
        {
            await ThrowIfServerVersionIsNotCompatible();

            return await Client.List<ArchivedEventFileResource>(
                await Repository.Link(CollectionLinkName).ConfigureAwait(false),
                new
                {
                    skip,
                    take
                }).ConfigureAwait(false);
        }
    }
}
