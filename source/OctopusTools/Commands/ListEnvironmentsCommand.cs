using System;
using OctopusTools.Client;
using log4net;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    [Command("list-environments", Description = "List all environments")]
    public class ListEnvironmentsCommand : ApiCommand
    {
        public ListEnvironmentsCommand(IOctopusSessionFactory session, ILog log) : base(session, log)
        {
        }

        protected override void Execute()
        {
            var environments = Session.ListEnvironments();
            
            Log.Info("Environments: " + environments.Count);
            foreach (var environment in environments)
            {
                Log.InfoFormat(" - {0} (ID: {1})", environment.Name, environment.Id);
            }
        }
    }
}
