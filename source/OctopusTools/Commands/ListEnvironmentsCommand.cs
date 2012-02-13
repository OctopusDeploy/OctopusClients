using System;
using OctopusTools.Client;
using log4net;

namespace OctopusTools.Commands
{
    public class ListEnvironmentsCommand : ApiCommand
    {
        public ListEnvironmentsCommand(IOctopusSessionFactory session, ILog log) : base(session, log)
        {
        }

        public override void Execute()
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
