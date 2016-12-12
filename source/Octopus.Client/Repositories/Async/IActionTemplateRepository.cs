using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        Task<List<ActionTemplateSearchResource>> Search();
    }
}