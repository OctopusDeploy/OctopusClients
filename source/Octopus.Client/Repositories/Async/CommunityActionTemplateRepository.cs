using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface ICommunityActionTemplateRepository : IGet<CommunityActionTemplateResource>
    {
        Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource);
        Task Install(CommunityActionTemplateResource resource);
        Task UpdateInstallation(CommunityActionTemplateResource resource);
    }

    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusAsyncRepository repository) : base(repository, "CommunityActionTemplates")
        {
        }

        public Task Install(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            ValidateRepositoryScope();

            var link = Repository.Scope.Type == RepositoryScope.RepositoryScopeType.Space
                ? baseLink.AppendSpaceId(Repository.Scope.SpaceId)
                : baseLink.ToString();
            return Client.Post(link);
        }

        public Task UpdateInstallation(CommunityActionTemplateResource resource)
        {
            ValidateRepositoryScope();
            return Client.Put(resource.Links["Installation"].AppendSpaceId(Repository.Scope.SpaceId));
        }

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            ValidateRepositoryScope();
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"].AppendSpaceId(Repository.Scope.SpaceId));
        }

        void ValidateRepositoryScope()
        {
            var isValidContext = Repository.Scope.Type == RepositoryScope.RepositoryScopeType.Space ||
                                 Repository.Scope.Type == RepositoryScope.RepositoryScopeType.Unspecified;
            if (!isValidContext)
                throw new MismatchSpaceContextException("You need to be within a single space context in order to execute this task");
        }
    }
}