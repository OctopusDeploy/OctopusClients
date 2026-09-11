using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<ActionTemplateSearchResource>> Search();
        Task<List<ActionTemplateSearchResource>> Search(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version);
        Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update);
        Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SetLogo(ActionTemplateResource resource, string fileName, Stream contents);
        Task SetLogo(ActionTemplateResource resource, string fileName, Stream contents, CancellationToken cancellationToken);
    }

    class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
    {
        public ActionTemplateRepository(IOctopusAsyncRepository repository) : base(repository, "ActionTemplates")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<ActionTemplateSearchResource>> Search()
            => Search(CancellationToken.None);

        public async Task<List<ActionTemplateSearchResource>> Search(CancellationToken cancellationToken)
        {
            return await Client.Get<List<ActionTemplateSearchResource>>(await Repository.Link("ActionTemplatesSearch").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<ActionTemplateCategoryResource>> Categories()
        {
            return await Client.Get<List<ActionTemplateCategoryResource>>(await Repository.Link("ActionTemplatesCategories").ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version)
            => GetVersion(resource, version, CancellationToken.None);

        public Task<ActionTemplateResource> GetVersion(ActionTemplateResource resource, int version, CancellationToken cancellationToken)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["Versions"], new { version }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update)
            => UpdateActions(actionTemplate, update, CancellationToken.None);

        public Task<ActionUpdateResultResource[]> UpdateActions(ActionTemplateResource actionTemplate, ActionsUpdateResource update, CancellationToken cancellationToken)
        {
            return Client.Post<ActionsUpdateResource, ActionUpdateResultResource[]>(actionTemplate.Links["ActionsUpdate"], update, new { actionTemplate.Id }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task SetLogo(ActionTemplateResource resource, string fileName, Stream contents)
            => SetLogo(resource, fileName, contents, CancellationToken.None);

        public Task SetLogo(ActionTemplateResource resource, string fileName, Stream contents, CancellationToken cancellationToken)
        {
            return Client.Post(resource.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false, cancellationToken);
        }
    }
}
