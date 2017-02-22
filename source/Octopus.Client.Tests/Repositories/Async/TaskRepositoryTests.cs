using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Tests.Repositories.Async
{
    public class TaskRepositoryTests
    {

        [Test]
        public void WaitForCompletionReportsProgress_ActionOverload()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            var repository = new TaskRepository(client);
            var taskResource = new TaskResource {Links = new LinkCollection() {{"Self", ""}}, State = TaskState.Queued};

            client.Get<TaskResource>(Arg.Any<string>()).Returns(c => Task.FromResult(taskResource));

            var callbackCount = 0;

            Action<TaskResource[]> action = t =>
            {
                t[0].Should().Be(taskResource);
                callbackCount++;
                taskResource = new TaskResource()
                {
                    State = callbackCount > 3 ? TaskState.Success : TaskState.Executing
                };
            };


            var wait = repository.WaitForCompletion(taskResource, pollIntervalSeconds: 1, timeoutAfterMinutes:1, interval: action);
            wait.Wait(TimeSpan.FromSeconds(30));

            callbackCount.Should().BeGreaterThan(3);
        }

        [Test]
        public void WaitForCompletionReportsProgress_TaskOverload()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            var repository = new TaskRepository(client);
            var taskResource = new TaskResource {Links = new LinkCollection() {{"Self", ""}}, State = TaskState.Queued };

            client.Get<TaskResource>(Arg.Any<string>()).Returns(c => Task.FromResult(taskResource));

            var callbackCount = 0;

            Func<TaskResource[], Task> func = t => Task.Run(() =>
            {
                t[0].Should().Be(taskResource);
                callbackCount++;
                taskResource = new TaskResource()
                {
                    State = callbackCount > 3 ? TaskState.Success : TaskState.Executing
                };
            });


            var wait = repository.WaitForCompletion(new [] { taskResource }, pollIntervalSeconds: 1, timeoutAfterMinutes: 1, interval: func);
            wait.Wait(TimeSpan.FromSeconds(30));

            callbackCount.Should().BeGreaterThan(3);
        }
    }
}