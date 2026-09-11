using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IArtifactRepository :
        IPaginate<ArtifactResource>,
        IGet<ArtifactResource>,
        ICreate<ArtifactResource>,
        IModify<ArtifactResource>,
        IDelete<ArtifactResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> GetContent(ArtifactResource artifact);
        Task<Stream> GetContent(ArtifactResource artifact, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task PutContent(ArtifactResource artifact, Stream contentStream);
        Task PutContent(ArtifactResource artifact, Stream contentStream, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ArtifactResource>> FindRegarding(IResource resource);
        Task<ResourceCollection<ArtifactResource>> FindRegarding(IResource resource, CancellationToken cancellationToken);
    }

    class ArtifactRepository : BasicRepository<ArtifactResource>, IArtifactRepository
    {
        public ArtifactRepository(IOctopusAsyncRepository repository)
            : base(repository, "Artifacts")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> GetContent(ArtifactResource artifact)
            => GetContent(artifact, CancellationToken.None);

        public Task<Stream> GetContent(ArtifactResource artifact, CancellationToken cancellationToken)
        {
            return Client.GetContent(artifact.Link("Content"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task PutContent(ArtifactResource artifact, Stream contentStream)
            => PutContent(artifact, contentStream, CancellationToken.None);

        public Task PutContent(ArtifactResource artifact, Stream contentStream, CancellationToken cancellationToken)
        {
            return Client.PutContent(artifact.Link("Content"), contentStream, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<ArtifactResource>> FindRegarding(IResource resource)
            => FindRegarding(resource, CancellationToken.None);

        public async Task<ResourceCollection<ArtifactResource>> FindRegarding(IResource resource, CancellationToken cancellationToken)
        {
            return await Client.List<ArtifactResource>(await Repository.Link("Artifacts").ConfigureAwait(false), new { regarding = resource.Id }, cancellationToken).ConfigureAwait(false);
        }
    }
}
