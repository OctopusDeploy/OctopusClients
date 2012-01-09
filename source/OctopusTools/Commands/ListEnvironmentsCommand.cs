using System;
using OctopusTools.Client;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public class ListEnvironmentsCommand : ApiCommand
    {
        public ListEnvironmentsCommand(IOctopusClientFactory client, ILog log) : base(client, log)
        {
        }

        public override void Execute()
        {
            var root = Client.Handshake().Execute();

            var environments = Client.List<DeploymentEnvironment>(root.Links["Environments"]).Execute();

            Log.Info("Environments: " + environments.Count);
            foreach (var environment in environments)
            {
                Log.InfoFormat(" - {0} (ID: {1})", environment.Name, environment.Id);
            }
        }
    }
}
