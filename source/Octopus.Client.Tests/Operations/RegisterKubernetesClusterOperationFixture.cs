using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Operations;

namespace Octopus.Client.Tests.Operations
{
    [TestFixture]
    public class RegisterKubernetesClusterOperationFixture
    {
        RegisterKubernetesClusterOperation operation;
        IOctopusClientFactory clientFactory;
        IOctopusAsyncClient client;
        OctopusServerEndpoint serverEndpoint;
        ResourceCollection<EnvironmentResource> environments;
        ResourceCollection<MachineResource> machines;
        ResourceCollection<MachinePolicyResource> machinePolicies;

        [SetUp]
        public void SetUp()
        {
            clientFactory = Substitute.For<IOctopusClientFactory>();
            client = Substitute.For<IOctopusAsyncClient>();
            clientFactory.CreateAsyncClient(Arg.Any<OctopusServerEndpoint>()).Returns(client);
            operation = new RegisterKubernetesClusterOperation(clientFactory);
            serverEndpoint = new OctopusServerEndpoint("http://octopus", "ABC123");

            environments = new ResourceCollection<EnvironmentResource>(new EnvironmentResource[0], LinkCollection.Self("/foo"));
            machines = new ResourceCollection<MachineResource>(new MachineResource[0], LinkCollection.Self("/foo"));
            machinePolicies = new ResourceCollection<MachinePolicyResource>(new MachinePolicyResource[0], LinkCollection.Self("/foo"));
            var rootDocument = new RootResource
            {
                ApiVersion = "3.0.0",
                Version = "2099.0.0",
                Links = LinkCollection.Self("/api")
                    .Add("Environments", "/api/environments")
                    .Add("Machines", "/api/machines")
                    .Add("MachinePolicies", "/api/machinepolicies")
                    .Add("CurrentUser", "/api/users/me")
                    .Add("SpaceHome", "/api/spaces")
            };

            client.Get<RootResource>(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(rootDocument);
            client.Repository.HasLink(Arg.Any<string>()).Returns(ci => rootDocument.HasLink(ci.Arg<string>()));
            client.Repository.Link(Arg.Any<string>()).Returns(ci => rootDocument.Link(ci.Arg<string>()));

            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<EnvironmentResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<EnvironmentResource>, bool>>()(environments));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<MachineResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<MachineResource>, bool>>()(machines));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<MachinePolicyResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<MachinePolicyResource>, bool>>()(machinePolicies));
        }

        [Test]
        public async Task ShouldCreateNewListeningKubernetesTentacle()
        {
            var machineResources = new List<MachineResource>();
            await client.Create("/api/machines", Arg.Do<MachineResource>(m => machineResources.Add(m)), Arg.Any<object>(), Arg.Any<CancellationToken>());

            environments.Items.Add(new EnvironmentResource {Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines")});
            environments.Items.Add(new EnvironmentResource {Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines")});

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] {"Production"};

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            machineResources.Should().ContainSingle().Which.Endpoint.Should().BeOfType<KubernetesTentacleEndpointResource>().Which.ShouldBeEquivalentTo(new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("ABCDEF", "https://mymachine.test.com:10930/")));
        }

        [Test]
        public async Task ShouldCreateNewPollingKubernetesTentacle()
        {
            var machineResources = new List<MachineResource>();
            await client.Create("/api/machines", Arg.Do<MachineResource>(m => machineResources.Add(m)), Arg.Any<object>(), Arg.Any<CancellationToken>());

            environments.Items.Add(new EnvironmentResource {Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines")});
            environments.Items.Add(new EnvironmentResource {Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines")});

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentacleActive;
            operation.EnvironmentNames = new[] {"Production"};
            operation.SubscriptionId = new Uri("poll://ckyhfyfkcbmzjl8sfgch/");

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            machineResources.Should().ContainSingle().Which.Endpoint.Should().BeOfType<KubernetesTentacleEndpointResource>().Which.ShouldBeEquivalentTo(new KubernetesTentacleEndpointResource(new PollingTentacleEndpointConfigurationResource("ABCDEF", "poll://ckyhfyfkcbmzjl8sfgch/")));
        }
    }
}