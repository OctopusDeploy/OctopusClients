using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.SpaceDefaultRetentionPolicies;
using Octopus.Client.Repositories;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class SpaceDefaultRetentionPolicyTests
    {
        private const string getUrl = "/api/{spaceId}/retentionpolicies?retentionType={retentionType}";
        private const string modifyUrl = "/api/{spaceId}/retentionpolicies/{id}";
        private string url;
        private object parameters;

        [TestCase("LifecycleTentacle")]
        [TestCase("LifecycleRelease")]
        [TestCase("RunbookRetention")]
        public async Task Get_UsesCorrectParameters(string retentionTypeValue)
        {
            var retentionType = new RetentionType(retentionTypeValue);
            await Task.CompletedTask;
            var client = SetupClient("2030.0.0");
            var repository = new SpaceDefaultRetentionPolicyRepository(client);

            client.Get<SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), Arg.Do<object>(x => parameters = x));

            var request = new GetDefaultRetentionPolicyByTypeRequest("Spaces-182737", retentionType);
            repository.Get(request);

            url.Should().Be(getUrl);
            parameters.Should().BeEquivalentTo(new { request.SpaceId, request.RetentionType });
        }

        [TestCase("LifecycleTentacle")]
        [TestCase("LifecycleRelease")]
        [TestCase("RunbookRetention")]
        public async Task AsyncGet_UsesCorrectParameters(string retentionTypeValue)
        {
            var retentionType = new RetentionType(retentionTypeValue);
            var asyncClient = SetupAsyncClient("2030.0.0");
            await asyncClient.Get<SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), Arg.Do<object>(x => parameters = x), Arg.Any<CancellationToken>());

            var asyncRepository = new Client.Repositories.Async.SpaceDefaultRetentionPolicyRepository(asyncClient);

            var request = new GetDefaultRetentionPolicyByTypeRequest("Spaces-182737", retentionType);
            var response = await asyncRepository.Get(request, CancellationToken.None);

            url.Should().Be(getUrl);
            parameters.Should().BeEquivalentTo(new { request.SpaceId, request.RetentionType });
        }

        [Test]
        public async Task Modify_LifecycleRelease_UsesCorrectParameters()
        {
            await Task.CompletedTask;

            var client = SetupClient("2030.0.0");
            var repository = new SpaceDefaultRetentionPolicyRepository(client);

            var command = CreateModifyLifecycleReleaseCommand();
            client.Update<ModifyDefaultLifecycleReleaseRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), command, Arg.Do<object>(x => parameters = x));
            repository.Modify(command);

            url.Should().Be(modifyUrl);
            parameters.Should().BeEquivalentTo(new { command.SpaceId, command.Id });
        }

        [Test]
        public async Task Modify_Runbook_UsesCorrectParameters()
        {
            await Task.CompletedTask;

            var client = SetupClient("2030.0.0");
            var repository = new SpaceDefaultRetentionPolicyRepository(client);

            var command = CreateModifyRunbookCommand();
            client.Update<ModifyDefaultRunbookRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), command, Arg.Do<object>(x => parameters = x));
            repository.Modify(command);

            url.Should().Be(modifyUrl);
            parameters.Should().BeEquivalentTo(new { command.SpaceId, command.Id });
        }

        [Test]
        public async Task AsyncModify_LifecycleRelease_UsesCorrectParameters()
        {
            var asyncClient = SetupAsyncClient("2030.0.0");
            var asyncRepository = new Client.Repositories.Async.SpaceDefaultRetentionPolicyRepository(asyncClient);

            var command = CreateModifyLifecycleReleaseCommand();

            await asyncClient.Update<ModifyDefaultLifecycleReleaseRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), command, Arg.Do<object>(x => parameters = x), Arg.Any<CancellationToken>());
            await asyncRepository.Modify(command, CancellationToken.None);

            url.Should().Be(modifyUrl);
            parameters.Should().BeEquivalentTo(new { command.SpaceId, command.Id });
        }

        [Test]
        public async Task AsyncModify_Runbook_UsesCorrectParameters()
        {
            var asyncClient = SetupAsyncClient("2030.0.0");
            var asyncRepository = new Client.Repositories.Async.SpaceDefaultRetentionPolicyRepository(asyncClient);

            var command = CreateModifyRunbookCommand();

            await asyncClient.Update<ModifyDefaultRunbookRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), command, Arg.Do<object>(x => parameters = x), Arg.Any<CancellationToken>());
            await asyncRepository.Modify(command, CancellationToken.None);

            url.Should().Be(modifyUrl);
            parameters.Should().BeEquivalentTo(new { command.SpaceId, command.Id });
        }

        [Test]
        public void AsyncModify_ThrowsWhenNotCompatible()
        {
            var asyncClient = SetupAsyncClient("2025.2.0");
            var asyncRepository = new Client.Repositories.Async.SpaceDefaultRetentionPolicyRepository(asyncClient);

            Assert.ThrowsAsync<NotSupportedException>(async () => await asyncRepository.Modify(new ModifyDefaultLifecycleReleaseRetentionPolicyCommand("Any-Id", "Any-SpaceId"), CancellationToken.None));
        }

        [Test]
        public async Task Modify_ThrowsWhenNotCompatible()
        {
            await Task.CompletedTask;

            var client = SetupClient("2025.2.0");
            var repository = new SpaceDefaultRetentionPolicyRepository(client);

            Assert.Throws<NotSupportedException>(() => repository.Modify(new ModifyDefaultLifecycleTentacleRetentionPolicyCommand("Any-Id", "Any-SpaceId")));
        }

        [Test]
        public void AsyncGet_ThrowsWhenNotCompatible()
        {
            var asyncClient = SetupAsyncClient("2025.2.0");
            var asyncRepository = new Client.Repositories.Async.SpaceDefaultRetentionPolicyRepository(asyncClient);

            Assert.ThrowsAsync<NotSupportedException>(async () => await asyncRepository.Get(new GetDefaultRetentionPolicyByTypeRequest(), CancellationToken.None));
        }

        [Test]
        public async Task Get_ThrowsWhenNotCompatible()
        {
            await Task.CompletedTask;

            var client = SetupClient("2025.2.0");
            var repository = new SpaceDefaultRetentionPolicyRepository(client);

            Assert.Throws<NotSupportedException>(() => repository.Get(new GetDefaultRetentionPolicyByTypeRequest()));
        }

        private ModifyDefaultLifecycleReleaseRetentionPolicyCommand CreateModifyLifecycleReleaseCommand()
        {
            return new ModifyDefaultLifecycleReleaseRetentionPolicyCommand("LifecycleRetentionRetentionDefault-Spaces-1873489", "Spaces-1873489");
        }

        private ModifyDefaultRunbookRetentionPolicyCommand CreateModifyRunbookCommand()
        {
            return new ModifyDefaultRunbookRetentionPolicyCommand("RunbookRetentionDefault-Spaces-999", "Spaces-989");
        }

        IOctopusAsyncClient SetupAsyncClient(string version)
        {
            var asyncClient = Substitute.For<IOctopusAsyncClient>();

            var rootDocument = new RootResource
            {
                ApiVersion = "3.0.0",
                Version = version,
            };
            asyncClient.Repository.LoadRootDocument(Arg.Any<CancellationToken>()).Returns(rootDocument);

            return asyncClient;
        }

        IOctopusClient SetupClient(string version)
        {
            var client = Substitute.For<IOctopusClient>();

            var rootDocument = new RootResource
            {
                ApiVersion = "3.0.0",
                Version = version,
            };

            client.Repository.LoadRootDocument().Returns(rootDocument);

            return client;
        }
    }
}
