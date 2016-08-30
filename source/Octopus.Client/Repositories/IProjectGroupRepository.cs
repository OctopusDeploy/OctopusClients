using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IProjectGroupRepository : IFindByName<ProjectGroupResource>, IGet<ProjectGroupResource>, ICreate<ProjectGroupResource>, IModify<ProjectGroupResource>, IDelete<ProjectGroupResource>, IGetAll<ProjectGroupResource>
    {
        List<ProjectResource> GetProjects(ProjectGroupResource projectGroup);
        ProjectGroupEditor CreateOrModify(string name);
        ProjectGroupEditor CreateOrModify(string name, string description);
    }
}