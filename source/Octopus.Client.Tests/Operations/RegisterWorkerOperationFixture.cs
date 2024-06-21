using System;
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
    public class RegisterWorkerOperationFixture
    {
        RegisterWorkerOperation operation;
        IOctopusClientFactory clientFactory;
        IOctopusAsyncClient client;
        OctopusServerEndpoint serverEndpoint;
        ResourceCollection<WorkerPoolResource> workerPools;
        ResourceCollection<WorkerResource> workers;

        [SetUp]
        public void SetUp()
        {
            clientFactory = Substitute.For<IOctopusClientFactory>();
            client = Substitute.For<IOctopusAsyncClient>();
            clientFactory.CreateAsyncClient(Arg.Any<OctopusServerEndpoint>()).Returns(client);
            operation = new RegisterWorkerOperation(clientFactory);
            serverEndpoint = new OctopusServerEndpoint("http://octopus", "ABC123");

            workerPools = new ResourceCollection<WorkerPoolResource>(Array.Empty<WorkerPoolResource>(), LinkCollection.Self("/foo"));
            workers = new ResourceCollection<WorkerResource>(Array.Empty<WorkerResource>(), LinkCollection.Self("/foo"));
            var rootDocument = new RootResource
            {
                ApiVersion = "3.0.0",
                Version = "2099.0.0",
                Links = LinkCollection.Self("/api")
                    .Add("Workers", "/api/workers")
                    .Add("WorkerPools", "/api/workerpools")
                    .Add("CurrentUser", "/api/users/me")
                    .Add("SpaceHome", "/api/spaces")
            };
            
            client.Get<RootResource>(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(rootDocument);
            client.Repository.HasLink(Arg.Any<string>()).Returns(ci => rootDocument.HasLink(ci.Arg<string>()));
            client.Repository.Link(Arg.Any<string>()).Returns(ci => rootDocument.Link(ci.Arg<string>()));

            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<WorkerPoolResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<WorkerPoolResource>, bool>>()(workerPools));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<WorkerResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<WorkerResource>, bool>>()(workers));
        }

        [Test]
        public async Task ShouldThrowIfWorkerPoolNameNotFound()
        {
            operation.WorkerPoolNames = new[] {"Atlantis"};
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pool named Atlantis on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }
        
        [Test]
        public async Task ShouldThrowIfWorkerPoolNameNotFoundEvenWhenWorkerPoolsHaveValid()
        {
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-2", Name = "MyWorkerPool", Links = LinkCollection.Self("/api/workerpools/workerpools-2") });
        
            operation.WorkerPoolNames = new[] {"Atlantis"};
            operation.WorkerPools = new[] {"MyWorkerPool"};
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pool named Atlantis on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }
        
        [Test]
        public async Task ShouldThrowIfWorkerPoolNotFoundBySlug()
        {
            operation.WorkerPools = new[] {"atlantis-slug"};
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pool with name, slug or Id atlantis-slug on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }
        
        [Test]
        public async Task ShouldThrowIfWorkerPoolNotFoundByIdOrSlug()
        {
            operation.WorkerPools = new[] {"WorkerPools-12345","atlantis-slug" };
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pools with names, slugs or Ids: WorkerPools-12345, atlantis-slug on the Octopus Server. Ensure the worker pools exist and you have permission to access them.");
        }
        
        [Test]
        public async Task ShouldThrowIfAnyWorkerPoolNotFound()
        {
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-2", Name = "MyWorkerPool", Links = LinkCollection.Self("/api/workerpools/WorkerPools-2") });
        
            operation.WorkerPools = new[] {"WorkerPools-2", "Atlantis", "ImaginaryPool"};
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pools with names, slugs or Ids: Atlantis, ImaginaryPool on the Octopus Server. Ensure the worker pools exist and you have permission to access them.");
        }
        
        [Test]
        public async Task ShouldCreateNewWorker()
        {
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-2", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});

            operation.WorkerPools = new[] { "workerpool-1", "PoolB" };
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyWorker";
            operation.TentacleHostname = "myworker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
        
            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);
        
            await client.Received().Create("/api/workers", Arg.Is<WorkerResource>(m =>
                    m.Name == "MyWorker"
                    && m.WorkerPoolIds.SetEquals(new []{ "WorkerPools-1", "WorkerPools-2" })
                    && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://myworker.test.com:10930/"),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }
    
        [Test]
        public async Task ShouldNotUpdateExistingWorkerWhenForceIsNotEnabled()
        {
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-2", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});
        
            workers.Items.Add(new WorkerResource {Id = "Worker-1", Name = "ExistingWorker", WorkerPoolIds = new ReferenceCollection(new[] {"workerpool-2"}), Links = LinkCollection.Self("/workers/Worker-1")});
        
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "ExistingWorker";
            operation.TentacleHostname = "newworker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPools = new[] { "workerpool-1", "PoolB" };
        
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint);
            await exec.Should().ThrowAsync<ArgumentException>();
        }
    
        [Test]
        public async Task ShouldUpdateExistingWorkerWhenForceIsEnabled()
        {
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-2", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});
        
            workers.Items.Add(new WorkerResource {Id = "Worker-1", Name = "ExistingWorker", WorkerPoolIds = new ReferenceCollection(new[] {"workerpool-2"}), Links = LinkCollection.Self("/workers/Worker-1")});
        
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "ExistingWorker";
            operation.TentacleHostname = "newworker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPools = new[] { "workerpool-1", "PoolB" };
            operation.AllowOverwrite = true;
        
            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);
            
            await client.Received().Update("/workers/Worker-1", Arg.Is<WorkerResource>(m =>
                        m.Name == "ExistingWorker"
                        && m.WorkerPoolIds.SetEquals(new []{ "WorkerPools-1", "WorkerPools-2" })
                        && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://newworker.test.com:10930/"),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }
        
        [Test]
        public async Task ShouldDeduplicateWorkerPools()
        {
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-slug-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-slug-2", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});
    
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyWorker";
            operation.TentacleHostname = "myworker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPoolNames = new[] {"PoolA"};
            operation.WorkerPools = new[] {"workerpool-slug-1", "workerpool-slug-2"};
    
            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);
    
            await client.Received().Create("/api/workers", Arg.Is<WorkerResource>(m =>
                        m.Name == "MyWorker"
                        && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://myworker.test.com:10930/"
                        && m.WorkerPoolIds.SetEquals(new []{"WorkerPools-1","WorkerPools-2"})),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }
    }
}