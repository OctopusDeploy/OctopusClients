using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusAsyncClient client) : base(client, "ActionTemplates")
        {
        }

        public Task<List<ActionTemplateSearchResource>> Search()
        {
            return Client.Get<List<ActionTemplateSearchResource>>(Client.RootDocument.Link("ActionTemplatesSearch"));
        }
    }
}