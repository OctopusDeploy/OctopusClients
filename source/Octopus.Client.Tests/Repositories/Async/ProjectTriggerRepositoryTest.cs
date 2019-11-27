using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Repositories.Async
{
    public class ProjectTriggerRepositoryTest
    {
        private const string NotSupportedOctopusVersion = "2019.9.3";

        [Test]
        [Ignore("restore when we bring back the ")]
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
            client.Repository.LoadRootDocument().Returns(new RootResource()
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
