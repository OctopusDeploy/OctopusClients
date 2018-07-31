using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.Environment;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    public class CreateEnvironmentCommandFixture : ApiCommandFixtureBase
    {
        private CreateEnvironmentCommand createEnvironmentCommand;

        [SetUp]
        public void Setup()
        {
            createEnvironmentCommand = new CreateEnvironmentCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
            //Repository.Environments.Create(Arg.Any<EnvironmentResource>()).Returns()
        }

        [Test]
        public async Task DefaultOutput_CreateNewEnvironment()
        {
            var newEnv = Guid.NewGuid().ToString();
            CommandLineArgs.Add($"--name={newEnv}");

            Repository.Environments.FindByName(Arg.Any<string>()).Returns((EnvironmentResource)null);
            Repository.Environments.Create(Arg.Any<EnvironmentResource>())
                .Returns(new EnvironmentResource { Id = Guid.NewGuid().ToString(), Name = newEnv });

            await createEnvironmentCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain($"Creating environment: {newEnv}");
        }

        [Test]
        public async Task JsonOutput_CreateNewEnvironment()
        {
            var newEnv = Guid.NewGuid().ToString();
            CommandLineArgs.Add($"--name={newEnv}");
            CommandLineArgs.Add("--outputFormat=json");

            Repository.Environments.FindByName(Arg.Any<string>()).Returns((EnvironmentResource)null);
            Repository.Environments.Create(Arg.Any<EnvironmentResource>())
                .Returns(new EnvironmentResource {Id = Guid.NewGuid().ToString(), Name = newEnv});

            await createEnvironmentCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            Console.WriteLine(logoutput);
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(newEnv);
        }

    }
}
