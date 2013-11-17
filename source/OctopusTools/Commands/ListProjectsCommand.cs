using Octopus.Client.Model;
using OctopusTools.Infrastructure;
using log4net;
using System;

namespace OctopusTools.Commands
{
    [Command("list-projects", Description = "Lists all projects")]
    public class ListProjectsCommand : ApiCommand
    {
        public ListProjectsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
        }

        protected override void Execute()
        {
            var projects = Repository.Projects.FindAll();

            Log.Info("Projects: " + projects.Count);

            foreach (var project in projects)
            {
                Log.InfoFormat(" - {0} (ID: {1})", project.Name, project.Id);
            }
        }
    }   
}
