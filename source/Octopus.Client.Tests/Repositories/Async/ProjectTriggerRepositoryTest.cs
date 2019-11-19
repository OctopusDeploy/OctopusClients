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

        [Test]
        public void ProjectTriggerRepositoryAsyncShouldOverrideAllBaseMethods()
        {
            var projectTriggerAsyncRepositoryMethods = GetPublicMethods(typeof(Client.Repositories.Async.ProjectTriggerRepository));
            var basicRepositoryAsyncMethods = GetPublicMethods(typeof(Client.Repositories.Async.BasicRepository<>));

            foreach (var basicRepositoryMethod in basicRepositoryAsyncMethods)
            {
                Assert.IsTrue(projectTriggerAsyncRepositoryMethods.Any(m => m.Name == basicRepositoryMethod.Name));
            }
        }

        [Test]
        public void ProjectTriggerRepositoryShouldOverrideAllBaseMethods()
        {
            var projectTriggerAsyncRepositoryMethods = GetPublicMethods(typeof(Client.Repositories.ProjectTriggerRepository));
            var basicRepositoryAsyncMethods = GetPublicMethods(typeof(Client.Repositories.BasicRepository<>));

            foreach (var basicRepositoryMethod in basicRepositoryAsyncMethods)
            {
                Assert.IsTrue(projectTriggerAsyncRepositoryMethods.Any(m => m.Name == basicRepositoryMethod.Name));
            }
        }

        IEnumerable<MethodInfo> GetPublicMethods(Type type)
        {
            return type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == type && !m.IsSpecialName);
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
