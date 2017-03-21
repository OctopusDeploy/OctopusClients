#if SYNC_CLIENT
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Repositories;
using Octopus.Client.Tests.Integration.OctopusClient;

namespace Octopus.Client.Tests.Repositories
{
    [TestFixture]
    public class MachineRepositoryFixture
    {
        [Test]
        public void GetTasksReturnsAllPages()
        {
            var octopusClient = new OctopusTestClient();
            var machine = new MachineResource { Links = new LinkCollection { { "TasksTemplate", "/api/machines/Machines-1/tasks{?skip}" } } };
            var repository = new MachineRepository(octopusClient);
            var tasks = repository.GetTasks(machine);

            Assert.That(tasks.Count, Is.EqualTo(139));
        }
    }
}
#endif
