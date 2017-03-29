using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        Task<List<ActionTemplateSearchResource>> Search();
        Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version);
        Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update);
    }

    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusAsyncClient client) : base(client, "ActionTemplates")
        {
        }

        public Task<List<ActionTemplateSearchResource>> Search()
        {
            return Client.Get<List<ActionTemplateSearchResource>>(Client.RootDocument.Link("ActionTemplatesSearch"));
        }

        public Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["Versions"], new { version });
        }

        public Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update)
        {
            return Client.Post<ActionsUpdateResource, ActionUpdateResultResource[]>(actionTemplate.Links["ActionsUpdate"], update, new { actionTemplate.Id });
        }
    }
}