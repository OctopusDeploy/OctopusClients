using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Model.EventRetention;

namespace Octopus.Client.Repositories.Async
{
    public interface IArchivedEventFileRepository :
        IDelete<ArchivedEventFileResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> GetContent(ArchivedEventFileResource archiveEventFile);
        Task<Stream> GetContent(ArchivedEventFileResource archiveEventFile, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ArchivedEventFileResource>> List(int skip = 0, int? take = null);
        Task<ResourceCollection<ArchivedEventFileResource>> List(CancellationToken cancellationToken, int skip = 0, int? take = null);
    }

    class ArchivedEventFileRepository : BasicRepository<ArchivedEventFileResource>, IArchivedEventFileRepository
    {
        public ArchivedEventFileRepository(IOctopusAsyncRepository repository) : base(repository, "ArchivedEventFiles")
        {
            MinimumCompatibleVersion("2022.3.8575");
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> GetContent(ArchivedEventFileResource archiveEventFile)
            => GetContent(archiveEventFile, CancellationToken.None);

        public async Task<Stream> GetContent(ArchivedEventFileResource archiveEventFile, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            return await Client.GetContent(archiveEventFile.Link("Self"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<ArchivedEventFileResource>> List(int skip = 0, int? take = null)
            => List(CancellationToken.None, skip, take);

        public async Task<ResourceCollection<ArchivedEventFileResource>> List(CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            return await Client.List<ArchivedEventFileResource>(
                await Repository.Link(CollectionLinkName).ConfigureAwait(false),
                new
                {
                    skip,
                    take
                },
                cancellationToken).ConfigureAwait(false);
        }
    }
}
