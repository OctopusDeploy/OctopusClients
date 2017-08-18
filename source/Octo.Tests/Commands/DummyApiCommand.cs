using System;
using System.Threading.Tasks;
using Castle.Core.Logging;
using NUnit.Framework;
using Octo.Commands;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using ILogger = Serilog.ILogger;

namespace Octopus.Cli.Tests.Commands
{
    public class DummyApiCommand : ApiCommand
    {
        string pill;
        public DummyApiCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Dummy");
            options.Add("pill=", "Red or Blue. Blue, the story ends. Red, stay in Wonderland and see how deep the rabbit hole goes.", v => pill = v);
            Log.Debug("Pill: " + pill);
        }

        protected override Task Execute()
        {
            return Task.WhenAll();
        }
    }

    public class DummyApiCommandWithFormattedOutputSupport : ApiCommand, ISupportFormattedOutput
    {
        public bool QueryCalled { get; set; }
        public bool PrintDefaultOutputCalled { get; set; }
        public bool PrintJsonOutputCalled { get; set; }
        public bool PrintXmlOutputCalled { get; set; }

        public DummyApiCommandWithFormattedOutputSupport(IOctopusClientFactory clientFactory, IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider) : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
            
        }

        public Task Query()
        {
            QueryCalled = true;
            return Task.WhenAll();
        }

        public void PrintDefaultOutput()
        {
            PrintDefaultOutputCalled = true;
        }

        public void PrintJsonOutput()
        {
            PrintJsonOutputCalled = true;
        }

        public void PrintXmlOutput()
        {
            PrintXmlOutputCalled = true;
        }
    }
}
