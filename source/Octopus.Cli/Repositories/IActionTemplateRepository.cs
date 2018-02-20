using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Cli.Repositories
{
    public interface IActionTemplateRepository
    {
        Task<ActionTemplateResource> Get(string idOrHref);
        Task<ActionTemplateResource> Create(ActionTemplateResource resource);
        Task<ActionTemplateResource> Modify(ActionTemplateResource resource);

        Task<ActionTemplateResource> FindByName(string name);
    }
}
