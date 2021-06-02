using System;
using System.IO;
using Octopus.Client.Extensibility;
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
    
    class ArtifactRepository : BasicRepository<ArtifactResource>, IArtifactRepository
    {
        public ArtifactRepository(IOctopusRepository repository)
            : base(repository, "Artifacts")
        {
        }

        public Stream GetContent(ArtifactResource artifact)
        {
            return Client.GetContent(artifact.Link("Content"));
        }

        public void PutContent(ArtifactResource artifact, Stream contentStream)
        {
            Client.PutContent(artifact.Link("Content"), contentStream);
        }

        public ResourceCollection<ArtifactResource> FindRegarding(IResource resource)
        {
            return Client.List<ArtifactResource>(Repository.Link("Artifacts"), new { regarding = resource.Id });
        }
    }
}