using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ITaskRepository : IPaginate<TaskResource>, IGet<TaskResource>, ICreate<TaskResource>
    {
        Task<TaskResource> ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null);
        Task<TaskResource> ExecuteCalamariUpdate(string description = null, string[] machineIds = null);
        Task<TaskResource> ExecuteBackup(string description = null);
        Task<TaskResource> ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null);
        Task<TaskResource> ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell");
        Task<TaskDetailsResource> GetDetails(TaskResource resource);
        Task<TaskResource> ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null);
        Task<TaskResource> ExecuteCommunityActionTemplatesSynchronisation(string description = null);
        Task<string> GetRawOutputLog(TaskResource resource);
        Task Rerun(TaskResource resource);
        Task Cancel(TaskResource resource);
        Task<List<TaskResource>> GetQueuedBehindTasks(TaskResource resource);
        Task WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], Task> interval = null);
    }

    class TaskRepository : BasicRepository<TaskResource>, ITaskRepository
    {
        public TaskRepository(IOctopusAsyncClient client)
            : base(client, "Tasks")
        {
        }

        public Task<TaskResource> ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.Health.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual health check" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.Health.Arguments.Timeout, TimeSpan.FromMinutes(timeoutAfterMinutes)},
                {BuiltInTasks.Health.Arguments.MachineTimeout, TimeSpan.FromMinutes(machineTimeoutAfterMinutes)},
                {BuiltInTasks.Health.Arguments.EnvironmentId, environmentId},
                {BuiltInTasks.Health.Arguments.MachineIds, machineIds}
            };
            return Create(resource);
        }

        public Task<TaskResource> ExecuteCalamariUpdate(string description = null, string[] machineIds = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.UpdateCalamari.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual Calamari update" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.UpdateCalamari.Arguments.MachineIds, machineIds }
            };
            return Create(resource);
        }

        public Task<TaskResource> ExecuteBackup(string description = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.Backup.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description;
            return Create(resource);
        }

        public Task<TaskResource> ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.Upgrade.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual upgrade" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.Upgrade.Arguments.EnvironmentId, environmentId},
                {BuiltInTasks.Upgrade.Arguments.MachineIds, machineIds}
            };
            return Create(resource);
        }

        public Task<TaskResource> ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell")
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.AdHocScript.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Run ad-hoc PowerShell script" : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                {BuiltInTasks.AdHocScript.Arguments.ScriptBody, scriptBody},
                {BuiltInTasks.AdHocScript.Arguments.Syntax, syntax}
            };
            return Create(resource);
        }

        public Task<TaskResource> ExecuteActionTemplate(ActionTemplateResource template, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null,
                                                        string[] environmentIds = null, string[] targetRoles = null, string description = null)
        {
            if (string.IsNullOrEmpty(template?.Id)) throw new ArgumentException("The step template was either null, or has no ID");

            var resource = new TaskResource();
            resource.Name = BuiltInTasks.AdHocScript.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Run step template: " + template.Name : description;
            resource.Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                    {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                    {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                    {BuiltInTasks.AdHocScript.Arguments.ActionTemplateId, template.Id},
                    {BuiltInTasks.AdHocScript.Arguments.Properties, properties}
                };
            return Create(resource);
        }

        public Task<TaskResource> ExecuteCommunityActionTemplatesSynchronisation(string description = null)
        {
            var resource = new TaskResource();
            resource.Name = BuiltInTasks.SyncCommunityActionTemplates.Name;
            resource.Description = description ?? "Run " + BuiltInTasks.SyncCommunityActionTemplates.Name;

            return Create(resource);
        }

        public Task<TaskDetailsResource> GetDetails(TaskResource resource)
        {
            return Client.Get<TaskDetailsResource>(resource.Link("Details"));
        }

        public Task<string> GetRawOutputLog(TaskResource resource)
        {
            return Client.Get<string>(resource.Link("Raw"));
        }

        public Task Rerun(TaskResource resource)
        {
            return Client.Post(resource.Link("Rerun"), (TaskResource)null);
        }
        
        public Task Cancel(TaskResource resource)
        {
            return Client.Post(resource.Link("Cancel"), (TaskResource)null);
        }

        public async Task<List<TaskResource>> GetQueuedBehindTasks(TaskResource resource)
        {
            var resources = new List<TaskResource>();

            await Client.Paginate<TaskResource>(resource.Link("QueuedBehind"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }).ConfigureAwait(false);

            return resources;
        }

        public Task WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
        {
            return WaitForCompletion(new[] { task }, pollIntervalSeconds, timeoutAfterMinutes, interval);
        }

        public Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
        {
            Func<TaskResource[], Task> taskInterval = null;
            if (interval != null)
                taskInterval = tr => Task.Run(() => interval(tr));

            return WaitForCompletion(tasks, pollIntervalSeconds, timeoutAfterMinutes, taskInterval);
        }

        public async Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], Task> interval = null)
        {
            var start = Stopwatch.StartNew();
            if (tasks == null || tasks.Length == 0)
                return;

            while (true)
            {
                var stillRunning = await Task.WhenAll(
                        tasks.Select(t => Client.Get<TaskResource>(t.Link("Self")))
                    )
                    .ConfigureAwait(false);

                if (interval != null)
                    await interval(stillRunning).ConfigureAwait(false);

                if (stillRunning.All(t => t.IsCompleted))
                    return;

                if (timeoutAfterMinutes > 0 && start.Elapsed.TotalMinutes > timeoutAfterMinutes)
                {
                    throw new TimeoutException($"One or more tasks did not complete before the timeout was reached. We waited {start.Elapsed.TotalMinutes:n1} minutes for the tasks to complete.");
                }

                Thread.Sleep(pollIntervalSeconds * 1000);
            }
        }
    }
}