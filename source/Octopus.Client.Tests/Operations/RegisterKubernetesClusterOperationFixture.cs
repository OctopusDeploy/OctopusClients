using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Exceptions;
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
        private ResourceCollection<ProxyResource> proxies;

        [SetUp]
        public void SetUp()
        {
            clientFactory = Substitute.For<IOctopusClientFactory>();
            client = Substitute.For<IOctopusAsyncClient>();
            clientFactory.CreateAsyncClient(Arg.Any<OctopusServerEndpoint>()).Returns(client);
            operation = new RegisterKubernetesClusterOperation(clientFactory);
            serverEndpoint = new OctopusServerEndpoint("http://octopus", "ABC123");

            proxies = new ResourceCollection<ProxyResource>(new ProxyResource[0], LinkCollection.Self("/foo"));
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
                    .Add("Proxies", "/api/proxies")
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
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<ProxyResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<ProxyResource>, bool>>()(proxies));
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

            machineResources.Should().ContainSingle().Which.Endpoint.Should().BeOfType<KubernetesTentacleEndpointResource>().Which.Should().BeEquivalentTo(new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("ABCDEF", "https://mymachine.test.com:10930/")));
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

            machineResources.Should().ContainSingle().Which.Endpoint.Should().BeOfType<KubernetesTentacleEndpointResource>().Which.Should().BeEquivalentTo(new KubernetesTentacleEndpointResource(new PollingTentacleEndpointConfigurationResource("ABCDEF", "poll://ckyhfyfkcbmzjl8sfgch/")));
        }

        [Test]
        public async Task ShouldOnlyUpdateEndpointConnectionConfigurationOnExistingMachine()
        {
            var updatedMachines = new List<MachineResource>();
            await client.Update("/machines/whatever/1", Arg.Do<MachineResource>(m => updatedMachines.Add(m)), Arg.Any<object>(), Arg.Any<CancellationToken>());

            environments.Items.Add(new EnvironmentResource {Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines")});
            environments.Items.Add(new EnvironmentResource {Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines")});

            machines.Items.Add(new MachineResource
            {
                Id = "machines/84", EnvironmentIds = new ReferenceCollection(new[] { "environments-1" }), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1"),
                Endpoint = new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("123456", "myMachine.test.com") { ProxyId = "proxy-2" })
            });

            proxies.Items.Add(new ProxyResource { Id = "proxy-1", Name = "MyNewProxy", Links = LinkCollection.Self("/api/proxies/proxy-1") });

            operation.MachineName = "Mymachine";
            operation.TentaclePort = 10930;
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentacleHostname = "my-new-machine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] {"Production"};
            operation.ProxyName = "MyNewProxy";

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            var thing = updatedMachines.Should().ContainSingle().Which;

            thing.Should().BeEquivalentTo(new MachineResource
            {
                Id = "machines/84",
                Name = "Mymachine",
                EnvironmentIds = new ReferenceCollection("environments-1"),
                Endpoint = new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("ABCDEF", "https://my-new-machine.test.com:10930/") {ProxyId = "proxy-1"}),
                Links = LinkCollection.Self("/machines/whatever/1")
            }, cfg => cfg.RespectingRuntimeTypes());
        }

        [Test]
        public async Task ShouldUpdateExistingMachineWhenForceIsEnabled()
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

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            await client.Received().Update("/machines/whatever/1", Arg.Is<MachineResource>(m =>
                m.Id == "machines/84"
                && m.Name == "Mymachine"
                && m.EnvironmentIds.First() == "environments-2"),
                Arg.Any<object>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldCreateWhenCantDeserializeMachines()
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

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                m.Name == "Mymachine"
                && ((KubernetesTentacleEndpointResource)m.Endpoint).TentacleEndpointConfiguration.Uri == "https://mymachine.test.com:10930/"
                && m.EnvironmentIds.First() == "environments-2"),
                Arg.Any<object>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldNotOverwriteMachinePolicyToNull()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            machines.Items.Add(new MachineResource { Id = "machines/84", MachinePolicyId = "MachinePolicies-1", EnvironmentIds = new ReferenceCollection(new[] { "environments-1" }), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };
            operation.AllowOverwrite = true;

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            await client.Received().Update("/machines/whatever/1", Arg.Is<MachineResource>(m =>
                m.Id == "machines/84"
                && m.Name == "Mymachine"
                && m.EnvironmentIds.First() == "environments-2"
                && m.MachinePolicyId == "MachinePolicies-1"),
                Arg.Any<object>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldOverwriteMachinePolicyWhenPassed()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            machines.Items.Add(new MachineResource { Id = "machines/84", MachinePolicyId = "MachinePolicies-1", EnvironmentIds = new ReferenceCollection(new[] { "environments-1" }), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1") });
            machinePolicies.Items.Add(new MachinePolicyResource {Id = "MachinePolicies-2", Name = "Machine Policy 2"});
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };
            operation.AllowOverwrite = true;
            operation.MachinePolicy = "Machine Policy 2";

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            await client.Received().Update("/machines/whatever/1", Arg.Is<MachineResource>(m =>
                m.Id == "machines/84"
                && m.Name == "Mymachine"
                && m.EnvironmentIds.First() == "environments-2"
                && m.MachinePolicyId == "MachinePolicies-2"),
                Arg.Any<object>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }
    }
}