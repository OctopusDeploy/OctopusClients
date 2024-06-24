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
    public class RegisterKubernetesWorkerOperationFixture
    {
        RegisterKubernetesWorkerOperation operation;
        IOctopusClientFactory clientFactory;
        IOctopusAsyncClient client;
        OctopusServerEndpoint serverEndpoint;
        ResourceCollection<WorkerResource> workers;
        ResourceCollection<WorkerPoolResource> workerPools;
        private ResourceCollection<ProxyResource> proxies;

        [SetUp]
        public void SetUp()
        {
            clientFactory = Substitute.For<IOctopusClientFactory>();
            client = Substitute.For<IOctopusAsyncClient>();
            clientFactory.CreateAsyncClient(Arg.Any<OctopusServerEndpoint>()).Returns(client);
            operation = new RegisterKubernetesWorkerOperation(clientFactory);
            serverEndpoint = new OctopusServerEndpoint("http://octopus", "ABC123");
            
            workers = new ResourceCollection<WorkerResource>(new WorkerResource[0], LinkCollection.Self("/foo"));
            workerPools = new ResourceCollection<WorkerPoolResource>(new WorkerPoolResource[0], LinkCollection.Self("/foo"));
            proxies = new ResourceCollection<ProxyResource>(new ProxyResource[0], LinkCollection.Self("/foo"));
            var rootDocument = new RootResource
            {
                ApiVersion = "3.0.0",
                Version = "2099.0.0",
                Links = LinkCollection.Self("/api")
                    .Add("Workers", "/api/workers")
                    .Add("WorkerPools", "/api/workerpools")
                    .Add("CurrentUser", "/api/users/me")
                    .Add("SpaceHome", "/api/spaces")
                    .Add("Proxies", "/api/proxies")
            };

            client.Get<RootResource>(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(rootDocument);
            client.Repository.HasLink(Arg.Any<string>()).Returns(ci => rootDocument.HasLink(ci.Arg<string>()));
            client.Repository.Link(Arg.Any<string>()).Returns(ci => rootDocument.Link(ci.Arg<string>()));

            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<WorkerResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<WorkerResource>, bool>>()(workers));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<WorkerPoolResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<WorkerPoolResource>, bool>>()(workerPools));
            client.When(x => x.Paginate(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<Func<ResourceCollection<ProxyResource>, bool>>(), Arg.Any<CancellationToken>()))
                .Do(ci => ci.Arg<Func<ResourceCollection<ProxyResource>, bool>>()(proxies));
        }

        [Test]
        public async Task ShouldCreateNewListeningKubernetesTentacle()
        {
            var workerResources = new List<WorkerResource>();
            await client.Create("/api/workers", Arg.Do<WorkerResource>(m => workerResources.Add(m)), Arg.Any<object>(), Arg.Any<CancellationToken>());

            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-2", WorkerPoolType = WorkerPoolType.DynamicWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyMachine";
            operation.TentacleHostname = "mymachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPools = new[] {"PoolA", "PoolB"};

            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            workerResources.Should().ContainSingle().Which.Endpoint.Should().BeOfType<KubernetesTentacleEndpointResource>().Which.Should().BeEquivalentTo(new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("ABCDEF", "https://mymachine.test.com:10930/")));
        }

        [Test]
        public async Task ShouldCreateNewPollingKubernetesTentacle()
        {
            var workerResources = new List<WorkerResource>();
            await client.Create("/api/workers", Arg.Do<WorkerResource>(m => workerResources.Add(m)), Arg.Any<object>(), Arg.Any<CancellationToken>());
        
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-2", WorkerPoolType = WorkerPoolType.DynamicWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});
        
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyOtherMachine";
            operation.TentacleHostname = "myothermachine.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentacleActive;
            operation.WorkerPools = new[] {"workerpool-2"};
            operation.SubscriptionId = new Uri("poll://ckyhfyfkcbmzjl8sfgch/");
        
            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);
        
            workerResources.Should().ContainSingle().Which.Endpoint.Should().BeOfType<KubernetesTentacleEndpointResource>().Which.Should().BeEquivalentTo(new KubernetesTentacleEndpointResource(new PollingTentacleEndpointConfigurationResource("ABCDEF", "poll://ckyhfyfkcbmzjl8sfgch/")));
        }

        [Test]
        public async Task ShouldOnlyUpdateEndpointConnectionConfigurationOnExistingWorker()
        {
            var updatedWorkers = await GetUpdatedWorkerAfterOperation(false);

            var worker = updatedWorkers.Should().ContainSingle().Which; 
            worker.Should().BeEquivalentTo(new
            {
                Id = "workers-84",
                Name = "MyWorker",
                WorkerPoolIds = new ReferenceCollection("WorkerPools-1"),
                Endpoint = new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("ABCDEF", "https://my-new-worker.test.com:10930/") {ProxyId = "proxy-1"}),
                Links = LinkCollection.Self("/machines/workers-84")
            });
        }
        
        [Test]
        public async Task ShouldUpdateExistingWorkerWhenForceIsEnabled()
        {
            var updatedWorkers = await GetUpdatedWorkerAfterOperation(true);
            
            var worker =updatedWorkers.Should().ContainSingle().Which; 
            worker.Should().BeEquivalentTo(new
            {
                Id = "workers-84",
                Name = "MyWorker",
                WorkerPoolIds = new ReferenceCollection("WorkerPools-2"),
                Endpoint = new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("ABCDEF", "https://my-new-worker.test.com:10930/") {ProxyId = "proxy-1"}),
                Links = LinkCollection.Self("/machines/workers-84")
            });
        }

        async Task<List<WorkerResource>> GetUpdatedWorkerAfterOperation(bool allowOverwrite)
        {
            var updatedWorkers = new List<WorkerResource>();
            await client.Update("/machines/workers-84", Arg.Do<WorkerResource>(m => updatedWorkers.Add(m)), Arg.Any<object>(), Arg.Any<CancellationToken>());
            
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1")});
            workerPools.Items.Add(new WorkerPoolResource {Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-2", WorkerPoolType = WorkerPoolType.DynamicWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2")});
        
            workers.Items.Add(new WorkerResource
            {
                Id = "workers-84", WorkerPoolIds = new ReferenceCollection(new[] { "WorkerPools-1" }), Name = "MyWorker", Links = LinkCollection.Self("/machines/workers-84"),
                Endpoint = new KubernetesTentacleEndpointResource(new ListeningTentacleEndpointConfigurationResource("123456", "myworker.test.com") { ProxyId = "proxy-2" })
            });
            
            proxies.Items.Add(new ProxyResource { Id = "proxy-1", Name = "MyNewProxy", Links = LinkCollection.Self("/api/proxies/proxy-1") });
        
            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyWorker";
            operation.TentacleHostname = "my-new-worker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPools = new[] {"PoolB"};
            operation.ProxyName = "MyNewProxy";
            operation.AllowOverwrite = allowOverwrite;
        
            await operation.ExecuteAsync(serverEndpoint).ConfigureAwait(false);

            return updatedWorkers;
        }
    }
}