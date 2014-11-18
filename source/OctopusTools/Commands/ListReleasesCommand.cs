using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Octopus.Client.Model;

namespace OctopusTools.Commands
{
    [Command("list-releases", Description = "List releases by project")]
    public class ListReleasesCommand : ApiCommand
    {
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ListReleasesCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
        }

        protected override void Execute()
        {
            var projectResources = new List<ProjectResource>();
            var projectsFilter = new string[0];
            if (projects.Count > 0)
            {
                Log.Debug("Loading projects...");
                var test = Repository.Projects.FindByNames(projects.ToArray());
                projectResources = Repository.Projects.FindByNames(projects.ToArray());
                projectsFilter = projectResources.Select(p => p.Id).ToArray();
            }

            Log.Debug("Loading releases...");
            var releases = Repository.Releases.FindMany(x =>
                {
                    return projectsFilter.Contains(x.ProjectId);
                });

            Log.InfoFormat("Releases: {0}", releases.Count);

            foreach (var project in projectResources)
            {
                Log.InfoFormat(" - Project: {0}", project.Name);
                
                foreach (var release in releases.Where(x => x.ProjectId == project.Id))
                {
                    var propertiesToLog = new List<string>();
                    propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(release));
                    foreach (var property in propertiesToLog)
                    {
                        Log.InfoFormat("    {0}", property);
                    }
                    Log.Info("");
                }
            }
        }
    }
}
