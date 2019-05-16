using System.Threading.Tasks;
using Octopus.Cli.Commands;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octo.Tests.Commands
{
    public class DummyApiCommand : ApiCommand
    {
        string pill;
        public DummyApiCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Dummy");
            options.Add("pill=", "Red or Blue. Blue, the story ends. Red, stay in Wonderland and see how deep the rabbit hole goes.", v => pill = v);
            commandOutputProvider.Debug("Pill: " + pill);
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

        public DummyApiCommandWithFormattedOutputSupport(IOctopusClientFactory clientFactory, IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider) : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            
        }

        public Task Request()
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
    }
}
