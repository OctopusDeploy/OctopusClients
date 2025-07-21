using System;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories.Async
{
    public class ProjectTriggerRepositoryTest
    {
        private const string NotSupportedOctopusVersion = "2019.9.3";

        [Test]
        public void ShouldThrowExceptionForOlderServerVersions()
        {
            var asyncClient = Substitute.For<IOctopusAsyncClient>();
            var client = Substitute.For<IOctopusClient>();
            SetupClient(client);
            SetupAsyncClient(asyncClient);

            var asyncRepository = new Client.Repositories.Async.ProjectTriggerRepository(asyncClient.Repository);
            Assert.ThrowsAsync<NotSupportedException>(async () => await asyncRepository.FindByName("fake trigger"));

            var repository = new Client.Repositories.ProjectTriggerRepository(client.Repository);
            Assert.Throws<NotSupportedException>(() => repository.FindByName("fake trigger"));
        }

        void SetupAsyncClient(IOctopusAsyncClient client)
        {
            client.Repository.LoadRootDocument(Arg.Any<CancellationToken>()).Returns(new RootResource()
            {
                Version = NotSupportedOctopusVersion
            });
        }

        void SetupClient(IOctopusClient client)
        {
            client.Repository.LoadRootDocument().Returns(new RootResource()
            {
                Version = NotSupportedOctopusVersion
            });
        }
    }
}
