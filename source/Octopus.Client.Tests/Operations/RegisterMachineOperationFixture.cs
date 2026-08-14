using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Operations;

namespace Octopus.Client.Tests.Operations
{
    [TestFixture]
    public class RegisterMachineOperationFixture
    {
        RegisterMachineOperation operation;
        IOctopusAsyncClient client;
        OctopusAsyncRepository repository;
        ResourceCollection<EnvironmentResource> environments;
        List<ParentEnvironmentV2Resource> parentEnvironments;
        List<EphemeralEnvironmentV2Resource> ephemeralEnvironments;
        ResourceCollection<TenantResource> tenants;
        ResourceCollection<MachineResource> machines;
        ResourceCollection<MachinePolicyResource> machinePolicies;

        [SetUp]
        public void SetUp()
        {
            client = Substitute.For<IOctopusAsyncClient>();
            operation = new RegisterMachineOperation();

            environments = new ResourceCollection<EnvironmentResource>(Array.Empty<EnvironmentResource>(), LinkCollection.Self("/foo"));
            parentEnvironments = new List<ParentEnvironmentV2Resource>();
            ephemeralEnvironments = new List<EphemeralEnvironmentV2Resource>();
            tenants = new ResourceCollection<TenantResource>(Array.Empty<TenantResource>(), LinkCollection.Self("/foo"));
            machines = new ResourceCollection<MachineResource>(Array.Empty<MachineResource>(), LinkCollection.Self("/foo"));
            machinePolicies = new ResourceCollection<MachinePolicyResource>(Array.Empty<MachinePolicyResource>(), LinkCollection.Self("/foo"));

            var rootDocument = new RootResource
            {
                ApiVersion = "3.0.0",
                Version = "2099.0.0",
                Links = LinkCollection.Self("/api")
            };

            var someSpace = new SpaceResource
            {
                Id = "Spaces-1",
                Links = LinkCollection.Self("/api/spaces/Spaces-1").Add("SpaceHome", "/api/spaces/Spaces-1/spaceHome")
            };

            var spaceRootDocument = new SpaceRootResource
            {
                Links = LinkCollection.Self("/api/spaces/Spaces-1/spaceHome")
                    .Add("Environments", "/api/environments")
                    .Add("Tenants", "/api/tenants")
                    .Add("Machines", "/api/machines")
                    .Add("MachinePolicies", "/api/machinepolicies")
            };

            client.Get<RootResource>(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(rootDocument);
            client.Get<SpaceRootResource>(Arg.Any<string>(), Arg.Any<object>()).Returns(spaceRootDocument);

            repository = new OctopusAsyncRepository(client, RepositoryScope.ForSpace(someSpace));

            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<EnvironmentResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<EnvironmentResource>, bool>>()(environments));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<TenantResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<TenantResource>, bool>>()(tenants));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<MachineResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<MachineResource>, bool>>()(machines));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<MachinePolicyResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<MachinePolicyResource>, bool>>()(machinePolicies));

            client.Get<PaginatedCollection<BaseEnvironmentV2Resource>>(Arg.Any<string>(), Arg.Any<object>())
                .Returns(ci =>
                {
                    var items = environments.Items
                        .Select(e => (BaseEnvironmentV2Resource)new StaticEnvironmentV2Resource { Id = e.Id, Name = e.Name, Slug = e.Slug })
                        .Concat(parentEnvironments)
                        .Concat(ephemeralEnvironments)
                        .ToList();
                    return new PaginatedCollection<BaseEnvironmentV2Resource> { TotalResults = items.Count, Items = items };
                });
        }

        [Test]
        public async Task ShouldThrowIfEnvironmentNameNotFound()
        {
            operation.EnvironmentNames = new[] { "Atlantis" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environment named Atlantis on the Octopus Server. Ensure the environment exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldThrowIfEnvironmentNameNotFoundEvenWhenEnvironmentsHasValid()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.EnvironmentNames = new[] { "Atlantis" };
            operation.Environments = new[] { "Production" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environment named Atlantis on the Octopus Server. Ensure the environment exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldThrowIfEnvironmentNotFoundBySlug()
        {
            operation.Environments = new[] { "atlantis-slug" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environment with name, slug or Id atlantis-slug on the Octopus Server. Ensure the environment exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldThrowIfEnvironmentNotFoundByIdOrSlug()
        {
            operation.Environments = new[] { "Environments-12345", "atlantis-slug" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environments with names, slugs or Ids: Environments-12345, atlantis-slug on the Octopus Server. Ensure the environments exist and you have permission to access them.");
        }

        [Test]
        public async Task ShouldThrowIfAnyEnvironmentNotFound()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.EnvironmentNames = new[] { "Production", "Atlantis", "Hyperborea" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environments named: Atlantis, Hyperborea on the Octopus Server. Ensure the environments exist and you have permission to access them.");
        }

        [Test]
        public async Task ShouldThrowIfAnyTenantNotFoundByNameIdOrSlug()
        {
            tenants.Items.Add(new TenantResource { Id = "Tenants-2", Name = "MyTenant", Slug = "my-tenant", Links = LinkCollection.Self("/api/environments/tenants-2") });

            operation.Tenants = new[] { "some-tenant", "Atlantis", "MyTenant" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the tenants with names, slugs or Ids: some-tenant, Atlantis on the Octopus Server. Ensure the tenants exist and you have permission to access them.");
        }

        [Test]
        public async Task ShouldNotThrowIfAllTenantsAreFoundByNameIdOrSlug()
        {
            tenants.Items.Add(new TenantResource { Id = "Tenants-1", Name = "SomeTenant", Slug = "some-tenant", Links = LinkCollection.Self("/api/environments/tenants-1") });
            tenants.Items.Add(new TenantResource { Id = "Tenants-2", Name = "MyTenant", Slug = "my-tenant", Links = LinkCollection.Self("/api/environments/tenants-2") });
            tenants.Items.Add(new TenantResource { Id = "Tenants-3", Name = "Atlantis", Slug = "atlantis", Links = LinkCollection.Self("/api/environments/tenants-3") });

            operation.Tenants = new[] { "Atlantis", "Tenants-1", "some-tenant" };
            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().NotThrowAsync();
        }

        [Test]
        public async Task ShouldCreateNewMachine()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                    m.Name == "Mymachine"
                    && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://mymachine.test.com:10930/"
                    && m.EnvironmentIds.First() == "environments-2"),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldNotUpdateExistingMachine()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            machines.Items.Add(new MachineResource { Id = "machines/84", EnvironmentIds = new ReferenceCollection(new[] { "environments-1" }), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };

            Func<Task> exec = () => operation.ExecuteAsync(repository);
            await exec.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task ShouldUpdateExistingMachineWhenForceIsEnabled()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            machines.Items.Add(new MachineResource { Id = "machines/84", EnvironmentIds = new ReferenceCollection(new[] { "environments-1" }), Name = "Mymachine", Links = LinkCollection.Self("/machines/whatever/1") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };
            operation.AllowOverwrite = true;

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

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

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                m.Name == "Mymachine"
                && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://mymachine.test.com:10930/"
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

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

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
            machinePolicies.Items.Add(new MachinePolicyResource { Id = "MachinePolicies-2", Name = "Machine Policy 2" });
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "Production" };
            operation.AllowOverwrite = true;
            operation.MachinePolicy = "Machine Policy 2";

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

            await client.Received().Update("/machines/whatever/1", Arg.Is<MachineResource>(m =>
                m.Id == "machines/84"
                && m.Name == "Mymachine"
                && m.EnvironmentIds.First() == "environments-2"
                && m.MachinePolicyId == "MachinePolicies-2"),
                Arg.Any<object>(), Arg.Any<CancellationToken>()).ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldDeduplicateEnvironments()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-1", Name = "UAT", Slug = "env-uat", Links = LinkCollection.Self("/api/environments/environments-1").Add("Machines", "/api/environments/environments-1/machines") });
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.EnvironmentNames = new[] { "UAT" };
            operation.Environments = new[] { "env-uat", "environments-2" };

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                        m.Name == "Mymachine"
                        && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://mymachine.test.com:10930/"
                        && m.EnvironmentIds.SequenceEqual(new[] { "environments-1", "environments-2" })),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldRegisterAgainstParentAndEphemeralEnvironmentsViaV2()
        {
            parentEnvironments.Add(new ParentEnvironmentV2Resource { Id = "environments-parent-1", Name = "Staging", Slug = "staging" });
            ephemeralEnvironments.Add(new EphemeralEnvironmentV2Resource { Id = "environments-eph-1", Name = "pr-123", Slug = "pr-123", ParentEnvironmentId = "environments-parent-1" });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.Environments = new[] { "Staging", "pr-123" };

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                    m.Name == "Mymachine"
                    && m.EnvironmentIds.SequenceEqual(new[] { "environments-parent-1", "environments-eph-1" })),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }

        [Test]
        public async Task ShouldFallBackToV1WhenV2EnvironmentsEndpointNotFound()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            client.Get<PaginatedCollection<BaseEnvironmentV2Resource>>(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>())
                .Throws(new OctopusResourceNotFoundException("Not found"));

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "Mymachine";
            operation.TentacleHostname = "Mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.Environments = new[] { "Production" };

            await operation.ExecuteAsync(repository).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                    m.Name == "Mymachine"
                    && m.EnvironmentIds.SequenceEqual(new[] { "environments-2" })),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }
    }
}
