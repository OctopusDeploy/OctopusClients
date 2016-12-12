using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IActionTemplateRepository : ICreate<ActionTemplateResource>, IModify<ActionTemplateResource>, IDelete<ActionTemplateResource>, IGet<ActionTemplateResource>, IFindByName<ActionTemplateResource>, IGetAll<ActionTemplateResource>
    {
        List<ActionTemplateSearchResource> Search();
    }
}