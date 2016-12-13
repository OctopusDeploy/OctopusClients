using System;
using System.IO;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IProjectRepository : IFindByName<ProjectResource>, IGet<ProjectResource>, ICreate<ProjectResource>, IModify<ProjectResource>, IDelete<ProjectResource>, IGetAll<ProjectResource>
    {
        ResourceCollection<ReleaseResource> GetReleases(ProjectResource project, int skip = 0);
        List<ReleaseResource> GetReleases(ProjectResource project);
        ReleaseResource GetReleaseByVersion(ProjectResource project, string version);
        ResourceCollection<ChannelResource> GetChannels(ProjectResource project);
        ResourceCollection<ProjectTriggerResource> GetTriggers(ProjectResource project);
        void SetLogo(ProjectResource project, string fileName, Stream contents);
        ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle);
        ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description);
    }
}