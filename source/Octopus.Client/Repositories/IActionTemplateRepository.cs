using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IActionTemplateRepository : IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>
    {
        
    }
}