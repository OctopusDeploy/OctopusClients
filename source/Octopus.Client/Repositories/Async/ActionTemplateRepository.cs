using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusAsyncClient client) : base(client, "ActionTemplates")
        {
        }
    }
}