using System;
using System.Collections.Generic;
using log4net;
using Octopus.Client.Model;

namespace OctopusTools.Commands
{
    public class ListEnvironmentsCommand : ApiCommand
    {
        public ListEnvironmentsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
        }

        protected override void Execute()
        {
            var environments = Repository.Environments.FindAll();
            Log.Info("Environments: " + environments.Count);

            foreach (var environment in environments)
            {
                Log.InfoFormat(" - {0} (ID: {1})", environment.Name, environment.Id);
            }
        }
    }
}