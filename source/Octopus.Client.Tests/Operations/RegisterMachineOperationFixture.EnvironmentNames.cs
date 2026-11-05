using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Tests.Operations
{
    // These tests deliberately cover the deprecated EnvironmentNames property, which is still shipped.
    // The Environments property is covered in RegisterMachineOperationFixture.cs; keep the two apart so new tests there don't silently use the deprecated property.
#pragma warning disable CS0618
    public partial class RegisterMachineOperationFixture
    {
        [Test]
        public async Task ShouldThrowIfEnvironmentNameNotFound()
        {
            operation.EnvironmentNames = new[] { "Atlantis" };
            Func<Task> exec = () => operation.ExecuteAsync(repository, CancellationToken.None);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environment named Atlantis on the Octopus Server. Ensure the environment exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldThrowIfEnvironmentNameNotFoundEvenWhenEnvironmentsHasValid()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.EnvironmentNames = new[] { "Atlantis" };
            operation.Environments = new[] { "Production" };
            Func<Task> exec = () => operation.ExecuteAsync(repository, CancellationToken.None);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environment named Atlantis on the Octopus Server. Ensure the environment exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldThrowIfAnyEnvironmentNotFound()
        {
            environments.Items.Add(new EnvironmentResource { Id = "environments-2", Name = "Production", Links = LinkCollection.Self("/api/environments/environments-2").Add("Machines", "/api/environments/environments-2/machines") });

            operation.EnvironmentNames = new[] { "Production", "Atlantis", "Hyperborea" };
            Func<Task> exec = () => operation.ExecuteAsync(repository, CancellationToken.None);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the environments named: Atlantis, Hyperborea on the Octopus Server. Ensure the environments exist and you have permission to access them.");
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

            await operation.ExecuteAsync(repository, CancellationToken.None).ConfigureAwait(false);

            await client.Received().Create("/api/machines", Arg.Is<MachineResource>(m =>
                        m.Name == "Mymachine"
                        && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://mymachine.test.com:10930/"
                        && m.EnvironmentIds.SequenceEqual(new[] { "environments-1", "environments-2" })),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }
    }
#pragma warning restore CS0618
}
