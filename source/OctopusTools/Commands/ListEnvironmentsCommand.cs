using System;
using OctopusTools.Client;
using OctopusTools.Model;
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
            var environments = Session.List<DeploymentEnvironment>(ServiceRoot.Links["Environments"]);

            Log.Info("Environments: " + environments.Count);
            foreach (var environment in environments)
            {
                Log.InfoFormat(" - {0} (ID: {1})", environment.Name, environment.Id);
            }
        }
    }
}
