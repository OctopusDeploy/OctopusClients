using System;
using System.IO;
using System.Reflection;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Integration.Repository
{
    public class MachineRepositoryTest : HttpIntegrationTestBase
    {
        public MachineRepositoryTest()
            : base(UrlPathPrefixBehaviour.UseNoPrefix) //as the canned responses have no prefix, it falls over if we try and isolate the tests with a different prefix
        {
            Get($"{TestRootPath}api/machines/Machines-1/tasks", parameters =>
            {
                string content = GetCannedResponse(parameters);
                return Response.AsText(content, "application/json");
            });
        }

        [Test]
        public void AsyncGetTasksReturnsAllPages()
        {
            var machine = new MachineResource { Links = new LinkCollection { { "TasksTemplate", $"{TestRootPath}api/machines/Machines-1/tasks{{?skip}}"} } };
            var repository = new MachineRepository(OctopusAsyncRepository.Create(AsyncClient).Result);
            var tasks = repository.GetTasks(machine).Result;

            Assert.That(tasks.Count, Is.EqualTo(139));
        }

#if SYNC_CLIENT
        [Test]
        public void SyncGetTasksReturnsAllPages()
        {
            var machine = new MachineResource { Links = new LinkCollection { { "TasksTemplate", $"{TestRootPath}api/machines/Machines-1/tasks{{?skip}}" } } };
            var repository = new Client.Repositories.MachineRepository(SyncClient.Repository);
            var tasks = repository.GetTasks(machine);

            Assert.That(tasks.Count, Is.EqualTo(139));
        }
#endif
    }
}