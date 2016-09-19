using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IProjectGroupRepository : IFindByName<ProjectGroupResource>, IGet<ProjectGroupResource>, ICreate<ProjectGroupResource>, IModify<ProjectGroupResource>, IDelete<ProjectGroupResource>, IGetAll<ProjectGroupResource>
    {
        Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup);
        Task<ProjectGroupEditor> CreateOrModify(string name);
        Task<ProjectGroupEditor> CreateOrModify(string name, string description);
    }
}