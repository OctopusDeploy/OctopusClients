using log4net;
using OctopusTools.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctopusTools.Commands
{
    public class ListProjectsCommand : ApiCommand
    {
        public ListProjectsCommand(IOctopusSessionFactory session, ILog log)
            : base(session, log)
        {
        }

        public override void Execute()
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
