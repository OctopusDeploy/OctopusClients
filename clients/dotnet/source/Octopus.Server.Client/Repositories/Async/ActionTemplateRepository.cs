﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        Task<List<ActionTemplateSearchResource>> Search();
        Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version);
        Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update);
        Task SetLogo(ActionTemplateResource resource, string fileName, Stream contents);
    }

    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusAsyncRepository repository) : base(repository, "ActionTemplates")
        {
        }

        public async Task<List<ActionTemplateSearchResource>> Search()
        {
            return await Client.Get<List<ActionTemplateSearchResource>>(await Repository.Link("ActionTemplatesSearch").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public async Task<List<ActionTemplateCategoryResource>> Categories()
        {
            return await Client.Get<List<ActionTemplateCategoryResource>>(await Repository.Link("ActionTemplatesCategories").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["Versions"], new { version });
        }

        public Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update)
        {
            return Client.Post<ActionsUpdateResource, ActionUpdateResultResource[]>(actionTemplate.Links["ActionsUpdate"], update, new { actionTemplate.Id });
        }
        
        public Task SetLogo(ActionTemplateResource resource, string fileName, Stream contents)
        {
            return Client.Post(resource.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
        }
    }
}