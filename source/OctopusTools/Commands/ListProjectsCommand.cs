using System;
using log4net;

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