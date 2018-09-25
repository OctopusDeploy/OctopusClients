using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface ITaskRepository : IPaginate<TaskResource>, IGet<TaskResource>, ICreate<TaskResource>, ICanExtendSpaceContext<ITaskRepository>
    {
        TaskResource ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null);
        TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null);
        TaskResource ExecuteBackup(string description = null);
        TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null);
        TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell");
        TaskResource ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null);
        TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null);
        List<TaskResource> GetAllActive(int pageSize = Int32.MaxValue);
        TaskDetailsResource GetDetails(TaskResource resource, bool? includeVerboseOutput = null, int? tail = null);
        string GetRawOutputLog(TaskResource resource);
        void Rerun(TaskResource resource);
        void Cancel(TaskResource resource);
        void ModifyState(TaskResource resource, TaskState newState, string reason);
        IReadOnlyList<TaskResource> GetQueuedBehindTasks(TaskResource resource);
        void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, TimeSpan? timeoutAfter = null, Action<TaskResource[]> interval = null);
    }

    class TaskRepository : MixedScopeBaseRepository<TaskResource>, ITaskRepository
    {
        public TaskRepository(IOctopusRepository repository)
            : base(repository, "Tasks")
        {
        }

        TaskRepository(IOctopusRepository repository, SpaceContext spaceContext)
            : base(repository, "Tasks", spaceContext)
        {
        }

        public TaskResource ExecuteHealthCheck(
            string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null, 
            string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
        {
            GetCurrentSpaceContext().EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
                SpaceId = GetCurrentSpaceContext().SpaceIds.Single(),
                Name = BuiltInTasks.Health.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Manual health check" : description,
                Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.Health.Arguments.Timeout, TimeSpan.FromMinutes(timeoutAfterMinutes)},
                    {BuiltInTasks.Health.Arguments.MachineTimeout, TimeSpan.FromMinutes(machineTimeoutAfterMinutes)},
                    {BuiltInTasks.Health.Arguments.EnvironmentId, environmentId},
                    {BuiltInTasks.Health.Arguments.WorkerpoolId, workerpoolId},
                    {BuiltInTasks.Health.Arguments.RestrictedTo, restrictTo},
                    {
                        BuiltInTasks.Health.Arguments.MachineIds,
                        machineIds?.Concat(workerIds ?? new string[0]).ToArray() ?? workerIds
                    }
                }
            };
            return Create(resource);
        }

        public TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null)
        {
            GetCurrentSpaceContext().EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
                SpaceId = GetCurrentSpaceContext().SpaceIds.Single(),
                Name = BuiltInTasks.UpdateCalamari.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Manual Calamari update" : description,
                Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.UpdateCalamari.Arguments.MachineIds, machineIds}
                }
            };
            return Create(resource);
        }

        public TaskResource ExecuteBackup(string description = null)
        {
            var resource = new TaskResource
            {
                Name = BuiltInTasks.Backup.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description
            };
            return CreateSystemTask(resource);
        }

        public TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
        {
            GetCurrentSpaceContext().EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
                SpaceId = GetCurrentSpaceContext().SpaceIds.Single(),
                Name = BuiltInTasks.Upgrade.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Manual upgrade" : description,
                Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.Upgrade.Arguments.EnvironmentId, environmentId},
                    {BuiltInTasks.Upgrade.Arguments.WorkerpoolId, workerpoolId},
                    {BuiltInTasks.Upgrade.Arguments.RestrictedTo, restrictTo},
                    {
                        BuiltInTasks.Upgrade.Arguments.MachineIds,
                        machineIds?.Concat(workerIds ?? new string[0]).ToArray() ?? workerIds
                    }
                }
            };
            return Create(resource);
        }

        public TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell")
        {
            GetCurrentSpaceContext().EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
                SpaceId = GetCurrentSpaceContext().SpaceIds.Single(),
                Name = BuiltInTasks.AdHocScript.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Run ad-hoc PowerShell script" : description,
                Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                    {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                    {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                    {BuiltInTasks.AdHocScript.Arguments.ScriptBody, scriptBody},
                    {BuiltInTasks.AdHocScript.Arguments.Syntax, syntax}
                }
            };
            return Create(resource);
        }

        public TaskResource ExecuteActionTemplate(ActionTemplateResource template, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null,
            string[] environmentIds = null, string[] targetRoles = null, string description = null)
        {
            if (string.IsNullOrEmpty(template?.Id)) throw new ArgumentException("The step template was either null, or has no ID");

            var resource = new TaskResource(){SpaceId = template.SpaceId};
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

        public TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null)
        {
            var resource = new TaskResource
            {
                Name = BuiltInTasks.SyncCommunityActionTemplates.Name,
                Description = description ?? "Run " + BuiltInTasks.SyncCommunityActionTemplates.Name
            };

            return CreateSystemTask(resource);
        }

        public TaskDetailsResource GetDetails(TaskResource resource, bool? includeVerboseOutput = null, int? tail = null)
        {
            var args = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (includeVerboseOutput.HasValue)
                args.Add("verbose", includeVerboseOutput.Value);

            if (tail.HasValue)
                args.Add("tail", tail.Value);
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, args);
            return Client.Get<TaskDetailsResource>(resource.Link("Details"), parameters);
        }

        public string GetRawOutputLog(TaskResource resource)
        {
            return Client.Get<string>(resource.Link("Raw"), AdditionalQueryParameters);
        }

        public void Rerun(TaskResource resource)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            Client.Post(resource.Link("Rerun"), (TaskResource)null);
        }

        public void Cancel(TaskResource resource)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            Client.Post(resource.Link("Cancel"), (TaskResource)null);
        }

        public void ModifyState(TaskResource resource, TaskState newState, string reason)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            Client.Post(resource.Link("State"), new { state = newState, reason = reason });
        }

        public IReadOnlyList<TaskResource> GetQueuedBehindTasks(TaskResource resource)
        {
            return Client.ListAll<TaskResource>(resource.Link("QueuedBehind"), AdditionalQueryParameters);
        }

        public void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
        {
            WaitForCompletion(new[] { task }, pollIntervalSeconds, timeoutAfterMinutes, interval);
        }

        public void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            => WaitForCompletion(tasks, pollIntervalSeconds, TimeSpan.FromMinutes(timeoutAfterMinutes), interval);

        public void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, TimeSpan? timeoutAfter = null, Action<TaskResource[]> interval = null)
        { 
            var start = Stopwatch.StartNew();
            if (tasks == null || tasks.Length == 0)
                return;

            while (true)
            {
                var stillRunning =
                (from task in tasks
                    let currentStatus = Client.Get<TaskResource>(task.Link("Self"), AdditionalQueryParameters)
                    select currentStatus).ToArray();

                interval?.Invoke(stillRunning);

                if (stillRunning.All(t => t.IsCompleted))
                    return;

                if (timeoutAfter.HasValue && timeoutAfter > TimeSpan.Zero && start.Elapsed > timeoutAfter)
                {
                    throw new TimeoutException(string.Format("One or more tasks did not complete before the timeout was reached. We waited {0:n1} minutes for the tasks to complete.", start.Elapsed.TotalMinutes));
                }

                Thread.Sleep(TimeSpan.FromSeconds(pollIntervalSeconds));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retreives all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        public List<TaskResource> GetAllActive(int pageSize = int.MaxValue) => FindAll(pathParameters: new { active = true, take = pageSize });

        public ITaskRepository Including(SpaceContext spaceContext)
        {
            return new TaskRepository(Repository, ExtendSpaceContext(spaceContext));
        }

        void EnsureTaskCanRunInTheCurrentContext(TaskResource task)
        {
            if (string.IsNullOrEmpty(task.SpaceId))
                return;
            if (!GetCurrentSpaceContext().SpaceIds.Contains(task.SpaceId))
                throw new MismatchSpaceContextException("You cannot perform this task in the current space context");
        }

        TaskResource CreateSystemTask(TaskResource task)
        {
            return Client.Create(Repository.Link(CollectionLinkName), task);
        }
    }
}