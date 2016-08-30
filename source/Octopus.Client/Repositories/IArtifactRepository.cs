using System;
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
        Stream GetContent(ArtifactResource artifact);
        void PutContent(ArtifactResource artifact, Stream contentStream);
        ResourceCollection<ArtifactResource> FindRegarding(IResource resource);
    }
}