using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        List<ActionTemplateSearchResource> Search();
        ActionTemplateResource GetVersion(ActionTemplateResource resource, int version);
        ActionUpdateResultResource[] UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update);
        void SetLogo(ActionTemplateResource resource, string fileName, Stream contents);
    }

    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusRepository repository) : base(repository, "ActionTemplates")
        {
        }

        public List<ActionTemplateSearchResource> Search()
        {
            return Client.Get<List<ActionTemplateSearchResource>>(Repository.Link("ActionTemplatesSearch"));
        }

        public ActionTemplateResource GetVersion(ActionTemplateResource resource, int version)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["Versions"], new {version});
        }

        public ActionUpdateResultResource[] UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update)
        {
            return Client.Post<ActionsUpdateResource, ActionUpdateResultResource[]>(actionTemplate.Links["ActionsUpdate"], update, new { actionTemplate.Id });
        }

        public void SetLogo(ActionTemplateResource resource, string fileName, Stream contents)
        {
            Client.Post(resource.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }
    }
}