using System;
using System.Diagnostics;
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
            var taskResource = new TaskResource { Links = new LinkCollection() { { "Self", "" } }, State = TaskState.Queued };

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


            var wait = repository.WaitForCompletion(taskResource, pollIntervalSeconds: 1, timeoutAfterMinutes: 1, interval: action);
            wait.Wait(TimeSpan.FromSeconds(30));

            callbackCount.Should().BeGreaterThan(3);
        }

        [Test]
        public void WaitForCompletionReportsProgress_TaskOverload()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            var repository = new TaskRepository(client);
            var taskResource = new TaskResource { Links = new LinkCollection() { { "Self", "" } }, State = TaskState.Queued };

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

            var wait = repository.WaitForCompletion(new[] { taskResource }, pollIntervalSeconds: 1, timeoutAfter: TimeSpan.FromSeconds(10), interval: func);
            wait.Wait(TimeSpan.FromSeconds(30));

            callbackCount.Should().BeGreaterThan(3);
        }

        [Test]
        public void WaitForCompletion_CancelsInATimelyManner()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            var repository = new TaskRepository(client);
            var taskResource = new TaskResource { Links = new LinkCollection() { { "Self", "" } }, State = TaskState.Queued };

            client.Get<TaskResource>(Arg.Any<string>()).Returns(c => Task.FromResult(taskResource));

            Action exec = () =>
            {
                try
                {
                    var wait = repository.WaitForCompletion(new[] {taskResource}, pollIntervalSeconds: 1,
                        timeoutAfter: TimeSpan.FromSeconds(3));
                    wait.Wait(TimeSpan.FromSeconds(30));
                }
                catch (AggregateException ae)
                {
                    throw ae.InnerExceptions[0];
                }
            };

            var sw = Stopwatch.StartNew();
            exec.ShouldThrow<TimeoutException>();
            sw.Stop();

            sw.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromSeconds(3), "Should run until the cancel time");
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5), "Should complete close to cancel time");
        }
    }
}