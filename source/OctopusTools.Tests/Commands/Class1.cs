using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class CleanEnvironmentCommandFixture : ApiCommandFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            listMachinesCommand = new CleanEnvironmentCommand(RepositoryFactory, Log, FileSystem);
        }

        CleanEnvironmentCommand listMachinesCommand;

        [Test]
        public void ShouldCleanEnvironmentWithEnvironmentAndStatusArgs()
        {
            CommandLineArgs.Add("-environment=Development");
            CommandLineArgs.Add("-status=Offline");

            Repository.Environments.FindByNames(Arg.Any<string[]>()).Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindMany(Arg.Any<Func<MachineResource, bool>>()).Returns(new List<MachineResource>
            {
                new MachineResource
                {
                    Name = "PC01234",
                    Id = "Machines-001",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource
                {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource
                {
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                }
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01466", MachineModelStatus.Offline, "Machines-002");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01996", MachineModelStatus.Offline, "Machines-003");
            Log.DidNotReceive().InfoFormat(" - {0} {1} (ID: {2})", "PC01234", MachineModelStatus.Online, "Machines-001");
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "Please specify an environment name using the parameter: --environment=XYZ")]
        public void ShouldNotCleanEnvironmentWithMissingEnvironmentArgs()
        {
            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "Please specify status using the parameter: --status=Offline")]
        public void ShouldNotCleanEnvironmentWithMissingStatusArgs()
        {
            CommandLineArgs.Add("-environment=Development");
            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }
    }
}
