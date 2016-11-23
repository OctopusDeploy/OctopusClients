using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusAsyncClient client) : base(client, "CommunityActionTemplates")
        {
        }
    }
}