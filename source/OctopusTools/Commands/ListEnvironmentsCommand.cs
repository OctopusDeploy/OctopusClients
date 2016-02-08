using System;
using log4net;
using OctopusTools.Util;

namespace OctopusTools.Commands
{
    [Command("list-environments", Description = "List environments")]
    public class ListEnvironmentsCommand : ApiCommand
    {

        public ListEnvironmentsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
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