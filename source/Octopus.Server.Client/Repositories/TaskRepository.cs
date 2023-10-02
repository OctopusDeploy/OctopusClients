using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Util;
using Octopus.Client.Validation;

namespace Octopus.Client.Repositories
{
    public interface ITaskRepository : IPaginate<TaskResource>, IGet<TaskResource>, ICreate<TaskResource>, ICanExtendSpaceContext<ITaskRepository>
    {
        TaskResource ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null);
        TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null);
        TaskResource ExecuteBackup(string description = null);
        TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null);
        TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell", BuiltInTasks.AdHocScript.TargetType? targetType = null);
        TaskResource ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, BuiltInTasks.AdHocScript.TargetType? targetType = null);
        TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null);
        
        /// <summary>
        /// Gets all the active tasks (optionally limited to pageSize)
        /// </summary>
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retrieves all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        List<TaskResource> GetAllActive(int pageSize = Int32.MaxValue);

        /// <summary>
        /// Returns all active tasks (optionally limited to pageSize) along with a count of all tasks in each status
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        TaskResourceCollection GetActiveWithSummary(int pageSize = int.MaxValue, int skip = 0);

        /// <summary>
        /// Returns all tasks (optionally limited to pageSize) along with a count of all tasks in each status
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        TaskResourceCollection GetAllWithSummary(int pageSize = int.MaxValue, int skip = 0);

        TaskDetailsResource GetDetails(TaskResource resource, bool? includeVerboseOutput = null, int? tail = null);
        string GetRawOutputLog(TaskResource resource);
        TaskTypeResource[] GetTaskTypes();
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

        TaskRepository(IOctopusRepository repository, SpaceContext userDefinedSpaceContext)
            : base(repository, "Tasks", userDefinedSpaceContext)
        {
        }

        public TaskResource ExecuteHealthCheck(
            string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null,
            string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
        {
            EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
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
            EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
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
            EnsureSystemContext();
            var resource = new TaskResource
            {
                Name = BuiltInTasks.Backup.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description
            };
            return CreateSystemTask(resource);
        }

        public TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
        {
            EnsureSingleSpaceContext();
            var resource = new TaskResource
            {
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

        public TaskResource ExecuteAdHocScript(
            string scriptBody,
            string[] machineIds = null,
            string[] environmentIds = null,
            string[] targetRoles = null,
            string description = null,
            string syntax = "PowerShell",
            BuiltInTasks.AdHocScript.TargetType? targetType = null)
        {
            EnsureSingleSpaceContext();
            EnsureValidTargetType(targetType);
            var resource = new TaskResource
            {
                Name = BuiltInTasks.AdHocScript.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Run ad-hoc PowerShell script" : description,
                Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                    {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                    {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                    {BuiltInTasks.AdHocScript.Arguments.ScriptBody, scriptBody},
                    {BuiltInTasks.AdHocScript.Arguments.Syntax, syntax},
                    {BuiltInTasks.AdHocScript.Arguments.TargetType, targetType},
                }
            };
            return Create(resource);
        }

        public TaskResource ExecuteActionTemplate(
            ActionTemplateResource template,
            Dictionary<string, PropertyValueResource> properties,
            string[] machineIds = null,
            string[] environmentIds = null,
            string[] targetRoles = null,
            string description = null,
            BuiltInTasks.AdHocScript.TargetType? targetType = null)
        {
            if (string.IsNullOrEmpty(template?.Id)) throw new ArgumentException("The step template was either null, or has no ID");
            EnsureValidTargetType(targetType);

            var resource = new TaskResource() {SpaceId = template.SpaceId};
            resource.Name = BuiltInTasks.AdHocScript.Name;
            resource.Description = string.IsNullOrWhiteSpace(description) ? "Run step template: " + template.Name : description;
            resource.Arguments = new Dictionary<string, object>
            {
                {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                {BuiltInTasks.AdHocScript.Arguments.ActionTemplateId, template.Id},
                {BuiltInTasks.AdHocScript.Arguments.Properties, properties},
                {BuiltInTasks.AdHocScript.Arguments.TargetType, targetType},
            };
            return Create(resource);
        }

        private void EnsureValidTargetType(BuiltInTasks.AdHocScript.TargetType? targetType)
        {
            if (targetType == BuiltInTasks.AdHocScript.TargetType.OctopusServer)
            {
                var minimumRequiredVersion = new SemanticVersion("2019.13.5");
                EnsureServerIsMinimumVersion(minimumRequiredVersion,
                    currentServerVersion => $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with the TargetType value of 'OctopusServer' for this API call. Please upgrade your Octopus Server to version '{minimumRequiredVersion}' or greater.");
            }
        }

        public TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null)
        {
            EnsureSystemContext();
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

        public TaskTypeResource[] GetTaskTypes()
        {
            return Client.Get<TaskTypeResource[]>(Client.Repository.LoadRootDocument().Links["TaskTypes"]);
        }

        public void Rerun(TaskResource resource)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            Client.Post(resource.Link("Rerun"), (TaskResource) null);
        }

        public void Cancel(TaskResource resource)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            Client.Post(resource.Link("Cancel"), (TaskResource) null);
        }

        public void ModifyState(TaskResource resource, TaskState newState, string reason)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            Client.Post(resource.Link("State"), new {state = newState, reason = reason});
        }

        public IReadOnlyList<TaskResource> GetQueuedBehindTasks(TaskResource resource)
        {
            return Client.ListAll<TaskResource>(resource.Link("QueuedBehind"), AdditionalQueryParameters);
        }

        public void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
        {
            WaitForCompletion(new[] {task}, pollIntervalSeconds, timeoutAfterMinutes, interval);
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

        public List<TaskResource> GetAllActive(int pageSize = int.MaxValue) => FindAll(pathParameters: new {active = true, take = pageSize});

        public TaskResourceCollection GetActiveWithSummary(int pageSize = int.MaxValue, int skip = 0)
            => Client.Get<TaskResourceCollection>(ResolveLink(), new {active = true, take = pageSize, skip});

        public TaskResourceCollection GetAllWithSummary(int pageSize = int.MaxValue, int skip = 0)
            => Client.Get<TaskResourceCollection>(ResolveLink(), new {take = pageSize, skip});

        public ITaskRepository UsingContext(SpaceContext userDefinedSpaceContext)
        {
            return new TaskRepository(Repository, userDefinedSpaceContext);
        }

        void EnsureTaskCanRunInTheCurrentContext(TaskResource task)
        {
            if (string.IsNullOrEmpty(task.SpaceId))
                return;
            var spaceContext = GetCurrentSpaceContext();

            spaceContext.ApplySpaceSelection(spaces =>
            {
                if (spaces.All(space => space.Id != task.SpaceId))
                {
                    throw new SpaceScopedOperationOutsideOfCurrentSpaceContextException(task.SpaceId, spaceContext);
                }
            }, () => { });
        }

        TaskResource CreateSystemTask(TaskResource task)
        {
            return Client.Create(Repository.Link(CollectionLinkName), task);
        }
    }
}