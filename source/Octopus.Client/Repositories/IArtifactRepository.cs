using System;
using System.Threading.Tasks;
using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IArtifactRepository :
        IPaginate<ArtifactResource>,
        IGet<ArtifactResource>,
        ICreate<ArtifactResource>,
        IModify<ArtifactResource>,
        IDelete<ArtifactResource>
    {
        Task<Stream> GetContent(ArtifactResource artifact);
        Task PutContent(ArtifactResource artifact, Stream contentStream);
        Task<ResourceCollection<ArtifactResource>> FindRegarding(IResource resource);
    }
}