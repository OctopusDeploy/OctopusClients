using System;
using Octopus.Client.Model;

namespace Octopus.Cli.Repositories
{
    public interface IActionTemplateRepository
    {
        ActionTemplateResource Get(string idOrHref);
        ActionTemplateResource Create(ActionTemplateResource resource);
        ActionTemplateResource Modify(ActionTemplateResource resource);

        ActionTemplateResource FindByName(string name);
    }
}
