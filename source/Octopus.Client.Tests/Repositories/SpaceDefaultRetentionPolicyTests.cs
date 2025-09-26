using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.SpaceDefaultRetentionPolicies;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class SpaceDefaultRetentionPolicyTests
    {
        private string url;
        private object parameters;

        private const string getUrl = "/api/{spaceId}/retentionpolicies?retentionType={retentionType}";
        private const string modifyUrl = "/api/{spaceId}/retentionpolicies/{id}";
        
        [Test]
        public async Task Get_UsesCorrectParameters()
        {
            await Task.CompletedTask;
            
            var client = SetupClient("2030.0.0");
            var repository = new Client.Repositories.SpaceDefaultRetentionPolicyRepository(client);
            
            client.Get<SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), Arg.Do<object>(x => parameters = x));

            var request = new GetDefaultRetentionPolicyByTypeRequest("Spaces-182737", RetentionType.LifecycleTentacle);
            repository.Get(request);

            url.Should().Be(getUrl);
            parameters.Should().BeEquivalentTo(new { SpaceId = request.SpaceId, RetentionType = request.RetentionType});
        }
        
        [Test]
        public async Task AsyncGet_UsesCorrectParameters()
        {
            var asyncClient = SetupAsyncClient("2030.0.0");
            await asyncClient.Get<SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), Arg.Do<object>(x => parameters = x), Arg.Any<CancellationToken>());

            var asyncRepository = new SpaceDefaultRetentionPolicyRepository(asyncClient);

            var request = new GetDefaultRetentionPolicyByTypeRequest("Spaces-182737", RetentionType.LifecycleRelease);
            var response = await asyncRepository.Get(request, CancellationToken.None);

            url.Should().Be(getUrl);
            parameters.Should().BeEquivalentTo(new { SpaceId = request.SpaceId, RetentionType = request.RetentionType});
        }

        [Test]
        public async Task Modify_UsesCorrectParameters()
        {
            await Task.CompletedTask;
            
            var client = SetupClient("2030.0.0");
            var repository = new Client.Repositories.SpaceDefaultRetentionPolicyRepository(client);

            var command = CreateModifyCommand();
            
            client.Update<ModifyDefaultLifecycleReleaseRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), command, Arg.Do<object>(x => parameters = x));
            repository.Modify(command);
            
            url.Should().Be(modifyUrl);
            parameters.Should().BeEquivalentTo(new { SpaceId = command.SpaceId, Id = command.Id});
        }

        [Test]
        public async Task AsyncModify_UsesCorrectParameters()
        {
            var asyncClient = SetupAsyncClient("2030.0.0");
            var asyncRepository = new SpaceDefaultRetentionPolicyRepository(asyncClient);

            var command = CreateModifyCommand();
            
            await asyncClient.Update<ModifyDefaultLifecycleReleaseRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(Arg.Do<string>(x => url = x), command, Arg.Do<object>(x => parameters = x), Arg.Any<CancellationToken>());
            await asyncRepository.Modify(command, CancellationToken.None);

            url.Should().Be(modifyUrl);
            parameters.Should().BeEquivalentTo(new { SpaceId = command.SpaceId, Id = command.Id});
        }
        
        [Test]
        public void AsyncModify_ThrowsWhenNotCompatible()
        {
            var asyncClient = SetupAsyncClient("2025.2.0");
            var asyncRepository = new SpaceDefaultRetentionPolicyRepository(asyncClient);

            Assert.ThrowsAsync<NotSupportedException>(async () => await asyncRepository.Modify(new ModifyDefaultLifecycleReleaseRetentionPolicyCommand("Any-Id", "Any-SpaceId"), CancellationToken.None));
        }
        
        [Test]
        public async Task Modify_ThrowsWhenNotCompatible()
        {
            await Task.CompletedTask;
            
            var client = SetupClient("2025.2.0");
            var repository = new Client.Repositories.SpaceDefaultRetentionPolicyRepository(client);

            Assert.Throws<NotSupportedException>(() => repository.Modify(new ModifyDefaultLifecycleTentacleRetentionPolicyCommand("Any-Id", "Any-SpaceId")));
        }
        
        [Test]
        public void AsyncGet_ThrowsWhenNotCompatible()
        {
            var asyncClient = SetupAsyncClient("2025.2.0");
            var asyncRepository = new SpaceDefaultRetentionPolicyRepository(asyncClient);

            Assert.ThrowsAsync<NotSupportedException>(async () => await asyncRepository.Get(new GetDefaultRetentionPolicyByTypeRequest(), CancellationToken.None));
        }
        
        [Test]
        public async Task Get_ThrowsWhenNotCompatible()
        {
            await Task.CompletedTask;
            
            var client = SetupClient("2025.2.0");
            var repository = new Client.Repositories.SpaceDefaultRetentionPolicyRepository(client);

            Assert.Throws<NotSupportedException>(() => repository.Get(new GetDefaultRetentionPolicyByTypeRequest()));
        }
        
        private ModifyDefaultLifecycleReleaseRetentionPolicyCommand CreateModifyCommand()
        {
            return new ModifyDefaultLifecycleReleaseRetentionPolicyCommand("LifecycleRetentionRetentionDefault-Spaces-1873489", "Spaces-1873489");
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
