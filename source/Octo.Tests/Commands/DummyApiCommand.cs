using System;
using System.Threading.Tasks;
using Serilog;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Tests.Commands
{
    public class DummyApiCommand : ApiCommand
    {
        string pill;
        public DummyApiCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Dummy");
            options.Add("pill=", "Red or Blue. Blue, the story ends. Red, stay in Wonderland and see how deep the rabbit hole goes.", v => pill = v);
            log.Debug("Pill: " + pill);
        }

        protected override Task Execute()
        {
            return Task.Run(() => Assert.Pass());
        }
    }
}
