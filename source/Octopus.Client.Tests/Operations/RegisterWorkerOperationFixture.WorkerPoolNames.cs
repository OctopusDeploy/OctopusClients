using System;
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
    // These tests deliberately cover the deprecated WorkerPoolNames property, which is still shipped.
    // The WorkerPools property is covered in RegisterWorkerOperationFixture.cs; keep the two apart so new tests there don't silently use the deprecated property.
#pragma warning disable CS0618
    public partial class RegisterWorkerOperationFixture
    {
        [Test]
        public async Task ShouldThrowIfWorkerPoolNameNotFound()
        {
            operation.WorkerPoolNames = new[] { "Atlantis" };
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint, CancellationToken.None);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pool named Atlantis on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }

        [Test]
        public void Sync_ShouldThrowIfWorkerPoolNameNotFound()
        {
            operation.WorkerPoolNames = new[] { "Atlantis" };
            Action exec = () => operation.Execute(serverEndpoint);
            exec.Should().Throw<ArgumentException>().WithMessage("Could not find the worker pool named Atlantis on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldThrowIfWorkerPoolNameNotFoundEvenWhenWorkerPoolsHaveValid()
        {
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-2", Name = "MyWorkerPool", Links = LinkCollection.Self("/api/workerpools/workerpools-2") });

            operation.WorkerPoolNames = new[] { "Atlantis" };
            operation.WorkerPools = new[] { "MyWorkerPool" };
            Func<Task> exec = () => operation.ExecuteAsync(serverEndpoint, CancellationToken.None);
            await exec.Should().ThrowAsync<ArgumentException>().WithMessage("Could not find the worker pool named Atlantis on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }

        [Test]
        public void Sync_ShouldThrowIfWorkerPoolNameNotFoundEvenWhenWorkerPoolsHaveValid()
        {
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-2", Name = "MyWorkerPool", Links = LinkCollection.Self("/api/workerpools/workerpools-2") });

            operation.WorkerPoolNames = new[] { "Atlantis" };
            operation.WorkerPools = new[] { "MyWorkerPool" };
            Action exec = () => operation.Execute(serverEndpoint);
            exec.Should().Throw<ArgumentException>().WithMessage("Could not find the worker pool named Atlantis on the Octopus Server. Ensure the worker pool exists and you have permission to access it.");
        }

        [Test]
        public async Task ShouldDeduplicateWorkerPools()
        {
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-slug-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1") });
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-slug-2", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyWorker";
            operation.TentacleHostname = "myworker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPoolNames = new[] { "PoolA" };
            operation.WorkerPools = new[] { "workerpool-slug-1", "workerpool-slug-2" };

            await operation.ExecuteAsync(serverEndpoint, CancellationToken.None).ConfigureAwait(false);

            await client.Received().Create("/api/workers", Arg.Is<WorkerResource>(m =>
                        m.Name == "MyWorker"
                        && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://myworker.test.com:10930/"
                        && m.WorkerPoolIds.SetEquals(new[] { "WorkerPools-1", "WorkerPools-2" })),
                    Arg.Any<object>(), Arg.Any<CancellationToken>())
                .ConfigureAwait(false);
        }

        [Test]
        public void Sync_ShouldDeduplicateWorkerPools()
        {
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-1", Name = "PoolA", Slug = "workerpool-slug-1", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-1") });
            workerPools.Items.Add(new WorkerPoolResource { Id = "WorkerPools-2", Name = "PoolB", Slug = "workerpool-slug-2", WorkerPoolType = WorkerPoolType.StaticWorkerPool, Links = LinkCollection.Self("/api/workerpools/WorkerPools-2") });

            operation.TentacleThumbprint = "ABCDEF";
            operation.TentaclePort = 10930;
            operation.MachineName = "MyWorker";
            operation.TentacleHostname = "myworker.test.com";
            operation.CommunicationStyle = CommunicationStyle.TentaclePassive;
            operation.WorkerPoolNames = new[] { "PoolA" };
            operation.WorkerPools = new[] { "workerpool-slug-1", "workerpool-slug-2" };

            operation.Execute(serverEndpoint);

            syncClient.Received().Create("/api/workers", Arg.Is<WorkerResource>(m =>
                        m.Name == "MyWorker"
                        && ((ListeningTentacleEndpointResource)m.Endpoint).Uri == "https://myworker.test.com:10930/"
                        && m.WorkerPoolIds.SetEquals(new[] { "WorkerPools-1", "WorkerPools-2" })));
        }
    }
#pragma warning restore CS0618
}
