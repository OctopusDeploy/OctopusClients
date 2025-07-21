using System.Collections.Concurrent;
using System.Linq;
using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Serialization;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace Octopus.Client.Tests.Integration.Repository
{
    public class MachineRepositoryTest : HttpIntegrationTestBase
    {
        private static ConcurrentDictionary<string, MachineResource> machines = new ConcurrentDictionary<string, MachineResource>();
        
        public MachineRepositoryTest()
            : base(UrlPathPrefixBehaviour.UseNoPrefix) //as the canned responses have no prefix, it falls over if we try and isolate the tests with a different prefix
        {
            Get($"{TestRootPath}api/machines/Machines-1/tasks", parameters =>
            {
                string content = GetCannedResponse(parameters);
                return Response.AsText(content, "application/json");
            });
            
            Get($"{TestRootPath}api/machines", parameters =>
            {
                var machinesArray = machines.ToArray().Select(x => x.Value).ToArray();
                if (machinesArray.Length == 1)
                    return Response.AsText(JsonConvert.SerializeObject(machinesArray[0],
                        JsonSerialization.GetDefaultSerializerSettings()), "application/json"); 
                
                return Response.AsText(JsonConvert.SerializeObject(machinesArray,
                    JsonSerialization.GetDefaultSerializerSettings()), "application/json"); 
            });
            
            Post($"{TestRootPath}api/machines", parameters =>
            {
                string requestAsString = Context.Request.Body.AsString();
                var machine = JsonConvert.DeserializeObject<MachineResource>(requestAsString,
                    JsonSerialization.GetDefaultSerializerSettings());
                machines.TryAdd(machine.Id, machine);
                return Response.AsText(requestAsString, "application/json");
            });
        }

        [Test]
        public void AsyncGetTasksReturnsAllPages()
        {
            var machine = new MachineResource { Links = new LinkCollection { { "TasksTemplate", $"{TestRootPath}api/machines/Machines-1/tasks{{?skip}}"} } };
            var repository = new MachineRepository(new OctopusAsyncRepository(AsyncClient));
            var tasks = repository.GetTasks(machine).Result;

            Assert.That(tasks.Count, Is.EqualTo(139));
        }

        [Test]
        public void SyncGetTasksReturnsAllPages()
        {
            var machine = new MachineResource { Links = new LinkCollection {{"TasksTemplate", $"{TestRootPath}api/machines/Machines-1/tasks{{?skip}}"}} };
            var repository = new Client.Repositories.MachineRepository(SyncClient.Repository);
            var tasks = repository.GetTasks(machine);

            Assert.That(tasks.Count, Is.EqualTo(139));
        }

        [Test]
        public void StepPackageEndpointInputs_IsSerializedAsObject()
        {
            var machine = new MachineResource
            {
                Id = "test-target",
                Endpoint = new StepPackageEndpointResource
                {
                    Id = "test-target",
                    Inputs = new { structureInput = "a", structuredInputB = new { accountId = "2" } }
                },
                Links = new LinkCollection {{"Machines", $"{TestRootPath}api/machines"}}
            };
            var repository = new Client.Repositories.MachineRepository(SyncClient.Repository);
            var result = repository.Create(machine);
            
            var stepPackageEndpoint = result.Endpoint as StepPackageEndpointResource;
            Assert.NotNull(stepPackageEndpoint);
            Assert.NotNull(stepPackageEndpoint.Inputs);
            Assert.IsNull(stepPackageEndpoint.Inputs as string);
        }
    }
}
