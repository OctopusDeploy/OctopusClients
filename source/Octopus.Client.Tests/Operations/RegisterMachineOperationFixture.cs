using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Operations;

namespace Octopus.Client.Tests.Operations
{
    [TestFixture]
    public class RegisterMachineOperationFixture
    {
        RegisterMachineOperation operation;
        IOctopusClientFactory clientFactory;
        IOctopusClient client;
        OctopusServerEndpoint serverEndpoint;
        ResourceCollection<EnvironmentResource> environments;
        ResourceCollection<MachineResource> machines;

        [SetUp]
        public void SetUp()
        {
            clientFactory = Substitute.For<IOctopusClientFactory>();
            client = Substitute.For<IOctopusClient>();
            clientFactory.CreateClient(Arg.Any<OctopusServerEndpoint>()).Returns(client);
            operation = new RegisterMachineOperation(clientFactory);
            serverEndpoint = new OctopusServerEndpoint("http://octopus", "ABC123");

            environments = new ResourceCollection<EnvironmentResource>(new EnvironmentResource[0], LinkCollection.Self("/foo"));
            machines = new ResourceCollection<MachineResource>(new MachineResource[0], LinkCollection.Self("/foo"));
            client.RootDocument.Returns(new RootResource {Links = LinkCollection.Self("/api").Add("Environments", "/api/environments").Add("Machines", "/api/machines")});

            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<EnvironmentResource>, bool>>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<EnvironmentResource>, bool>>()(environments));

            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<MachineResource>, bool>>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<MachineResource>, bool>>()(machines));

            client.List<MachineResource>(Arg.Any<string>(), Arg.Any<object>()).Returns(machines);
        }

        [Test]
        public void ShouldThrowIfEnvironmentNotFound()
        {
            operation.EnvironmentNames = new[] {"Atlantis"};

            var ex = Assert.Throws<ArgumentException>(() => operation.Execute(serverEndpoint));
            Assert.That(ex.Message, Is.EqualTo("Could not find the environment Atlantis on the Octopus server."));
        }
#pragma warning disable 618
        [Test]
        public void ShouldCreateNewMachine()
        {
            environments.Items.Add(new EnvironmentResource {Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines")});
            environments.Items.Add(new EnvironmentResource {Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines")});

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] {"Production"};

            operation.Execute(serverEndpoint);

            client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                m.Name == "Mymachine"
                    && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://mymachine.test.com:10930/"
                    && m.EnvironmentIds.First() == "environments-2"));
        }

        [Test]
        public void ShouldNotUpdateExistingMachine()
        {
            environments.Items.Add(new EnvironmentResource {Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines")});
            environments.Items.Add(new EnvironmentResource {Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines")});

            machines.Items.Add(new MachineResource {Id = "machines/84", EnvironmentIds = new ReferenceCollection(new[] {"environments-1"}), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1")});

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] {"Production"};

            Assert.Throws<ArgumentException>(() => operation.Execute(serverEndpoint));
        }

        [Test]
        public void ShouldUpdateExistingMachineWhenForceIsEnabled()
        {
            environments.Items.Add(new EnvironmentResource {Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines")});
            environments.Items.Add(new EnvironmentResource {Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines")});

            machines.Items.Add(new MachineResource {Id = "machines/84", EnvironmentIds = new ReferenceCollection(new[] {"environments-1"}), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1")});

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] {"Production"};
            operation.AllowOverwrite = true;

            operation.Execute(serverEndpoint);

            client.Received().Update("/machines/whatever/1", Arg.Is<MachineResource>(m =>
                m.Id == "machines/84"
                    && m.Name == "Mymachine"
                    && m.EnvironmentIds.First() == "environments-2"));
        }

        [Test]
        public void ShouldCreateWhenCantDeserializeMachines()
        {
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<MachineResource>, bool>>()))
                .Throw(new OctopusDeserializationException(1, "Can not deserialize"));

            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };

            operation.Execute(serverEndpoint);

            client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                m.Name == "Mymachine"
                    && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://mymachine.test.com:10930/"
                    && m.EnvironmentIds.First() == "environments-2"));
        }
    }
}