using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Tests.Integration.OctopusClient;

namespace Octopus.Client.Tests.Repositories.Async
{
    [TestFixture]
    public class MachineRepositoryFixture
    {
        [Test]
        public void GetTasksReturnsAllPages()
        {
            var octopusClient = new OctopusAsyncTestClient();
            var machine = new MachineResource { Links = new LinkCollection { { "TasksTemplate", "/api/machines/Machines-1/tasks{?skip}" } } };
            var repository = new MachineRepository(octopusClient);
            var tasks = repository.GetTasks(machine);

            Assert.That(tasks.Result.Count, Is.EqualTo(139));
        }
    }
}
