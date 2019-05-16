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
    
    class ProjectGroupRepository : BasicRepository<ProjectGroupResource>, IProjectGroupRepository
    {
        public ProjectGroupRepository(IOctopusRepository repository)
            : base(repository, "ProjectGroups")
        {
        }

        public List<ProjectResource> GetProjects(ProjectGroupResource projectGroup)
        {
            var resources = new List<ProjectResource>();

            Client.Paginate<ProjectResource>(projectGroup.Link("Projects"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public ProjectGroupEditor CreateOrModify(string name)
        {
            return new ProjectGroupEditor(this).CreateOrModify(name);
        }

        public ProjectGroupEditor CreateOrModify(string name, string description)
        {
            return new ProjectGroupEditor(this).CreateOrModify(name, description);
        }
    }
}