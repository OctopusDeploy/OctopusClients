using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Repositories
{
    public class ActionTemplateRepository : IActionTemplateRepository
    {
        private readonly IOctopusAsyncClient client;

        public ActionTemplateRepository(IOctopusAsyncClient client)
        {
            this.client = client;
            
        }

        public async Task<ActionTemplateResource> Get(string idOrHref)
        {
            if (string.IsNullOrWhiteSpace(idOrHref)) return null;
            var templatesPath = client.RootDocument.Link("ActionTemplates");
            return await client.Get<ActionTemplateResource>(templatesPath, new { id = idOrHref }).ConfigureAwait(false);
        }

        public async Task<ActionTemplateResource> Create(ActionTemplateResource resource)
        {
            var templatesPath = client.RootDocument.Link("ActionTemplates");
            return await client.Create(templatesPath, resource).ConfigureAwait(false);
        }

        public Task<ActionTemplateResource> Modify(ActionTemplateResource resource)
        {
            return client.Update(resource.Links["Self"], resource);
        }

        public async Task<ActionTemplateResource> FindByName(string name)
        {
            ActionTemplateResource template = null;

            name = (name ?? string.Empty).Trim();
            var templatesPath = client.RootDocument.Link("ActionTemplates");
            await client.Paginate<ActionTemplateResource>(templatesPath, page =>
            {
                template = page.Items.FirstOrDefault(t => string.Equals((t.Name ?? string.Empty), name, StringComparison.OrdinalIgnoreCase));
                // If no matching template was found, then we need to try the next page.
                return (template == null);
            })
            .ConfigureAwait(false);

            return template;
        }
    }
}
