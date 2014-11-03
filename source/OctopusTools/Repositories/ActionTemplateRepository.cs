using System;
using System.Linq;
using Octopus.Client;
using Octopus.Client.Model;

namespace OctopusTools.Repositories
{
    public class ActionTemplateRepository : IActionTemplateRepository
    {
        private readonly IOctopusClient client;
        private readonly string templatesPath;

        public ActionTemplateRepository(IOctopusClient client)
        {
            this.client = client;
            templatesPath = client.RootDocument.Link("ActionTemplates");
        }

        public ActionTemplateResource Get(string idOrHref)
        {
            if (string.IsNullOrWhiteSpace(idOrHref)) return null;
            return client.Get<ActionTemplateResource>(templatesPath, new { id = idOrHref });
        }

        public ActionTemplateResource Create(ActionTemplateResource resource)
        {
            return client.Create(templatesPath, resource);
        }

        public ActionTemplateResource Modify(ActionTemplateResource resource)
        {
            return client.Update(resource.Links["Self"], resource);
        }

        public ActionTemplateResource FindByName(string name)
        {
            ActionTemplateResource template = null;

            name = (name ?? string.Empty).Trim();
            client.Paginate<ActionTemplateResource>(templatesPath, page =>
            {
                template = page.Items.FirstOrDefault(t => string.Equals((t.Name ?? string.Empty), name, StringComparison.OrdinalIgnoreCase));
                // If no matching template was found, then we need to try the next page.
                return (template == null);
            });

            return template;
        }
    }
}
