using log4net;
using OctopusTools.Client;
using System;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("list-projects", Description = "List all projects")]
    public class ListProjectsCommand : ApiCommand
    {
        public ListProjectsCommand(IOctopusSessionFactory session, ILog log)
            : base(session, log)
        {
        }

        protected override void Execute()
        {
            var projects = Session.ListProjects();

            Log.Info("Projects: " + projects.Count);
            foreach (var project in projects)
            {
                Log.InfoFormat(" - {0} (ID: {1})", project.Name, project.Id);
            }
        }
    }
}
