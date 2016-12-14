using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        List<ActionTemplateSearchResource> Search();
    }
    
    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusClient client) : base(client, "ActionTemplates")
        {
        }

        public List<ActionTemplateSearchResource> Search()
        {
            return Client.Get<List<ActionTemplateSearchResource>>(Client.RootDocument.Link("ActionTemplatesSearch"));
        }
    }
}