using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;
using Octopus.Client.Validation;

namespace Octopus.Client.Repositories.Async
{
    public interface ITaskRepository : IPaginate<TaskResource>, IGet<TaskResource>, ICreate<TaskResource>, ICanExtendSpaceContext<ITaskRepository>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null);
        Task<TaskResource> ExecuteHealthCheck(CancellationToken cancellationToken, string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteCalamariUpdate(string description = null, string[] machineIds = null);
        Task<TaskResource> ExecuteCalamariUpdate(CancellationToken cancellationToken, string description = null, string[] machineIds = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteBackup(string description = null);
        Task<TaskResource> ExecuteBackup(CancellationToken cancellationToken, string description = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpooltId = null, string[] workerIds = null);
        Task<TaskResource> ExecuteTentacleUpgrade(CancellationToken cancellationToken, string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpooltId = null, string[] workerIds = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell", BuiltInTasks.AdHocScript.TargetType? targetType = null);
        Task<TaskResource> ExecuteAdHocScript(string scriptBody, CancellationToken cancellationToken, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell", BuiltInTasks.AdHocScript.TargetType? targetType = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskDetailsResource> GetDetails(TaskResource resource, bool? includeVerboseOutput = null, int? tail = null);
        Task<TaskDetailsResource> GetDetails(TaskResource resource, CancellationToken cancellationToken, bool? includeVerboseOutput = null, int? tail = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, BuiltInTasks.AdHocScript.TargetType? targetType = null);
        Task<TaskResource> ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, CancellationToken cancellationToken, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, BuiltInTasks.AdHocScript.TargetType? targetType = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResource> ExecuteCommunityActionTemplatesSynchronisation(string description = null);
        Task<TaskResource> ExecuteCommunityActionTemplatesSynchronisation(CancellationToken cancellationToken, string description = null);
        
        /// <summary>
        /// Gets all the active tasks (optionally limited to pageSize)
        /// </summary>
        /// <param name="pageSize">Number of items per page, setting to less than the total items still retreives all items, but uses multiple requests reducing memory load on the server</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TaskResource>> GetAllActive(int pageSize = int.MaxValue);
        Task<List<TaskResource>> GetAllActive(CancellationToken cancellationToken, int pageSize = int.MaxValue);
        
        /// <summary>
        /// Returns all active tasks (optionally limited to pageSize) along with a count of all tasks in each status
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResourceCollection> GetActiveWithSummary(int pageSize = int.MaxValue, int skip = 0);
        Task<TaskResourceCollection> GetActiveWithSummary(CancellationToken cancellationToken, int pageSize = int.MaxValue, int skip = 0);

        /// <summary>
        /// Returns all tasks (optionally limited to pageSize) along with a count of all tasks in each status
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskResourceCollection> GetAllWithSummary(int pageSize = int.MaxValue, int skip = 0);
        Task<TaskResourceCollection> GetAllWithSummary(CancellationToken cancellationToken, int pageSize = int.MaxValue, int skip = 0);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<string> GetRawOutputLog(TaskResource resource);
        Task<string> GetRawOutputLog(TaskResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TaskTypeResource[]> GetTaskTypes();
        Task<TaskTypeResource[]> GetTaskTypes(CancellationToken cancellationToken);
        
        /// <summary>
        /// Moves queued task to the top of the Task Queue
        /// </summary>
        Task Prioritize(TaskResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Rerun(TaskResource resource);
        Task Rerun(TaskResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Cancel(TaskResource resource);
        Task Cancel(TaskResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task ModifyState(TaskResource resource, TaskState newState, string reason);
        Task ModifyState(TaskResource resource, TaskState newState, string reason, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<TaskResource>> GetQueuedBehindTasks(TaskResource resource);
        Task<IReadOnlyList<TaskResource>> GetQueuedBehindTasks(TaskResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        Task WaitForCompletion(TaskResource task, CancellationToken cancellationToken, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[], CancellationToken> interval = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        Task WaitForCompletion(TaskResource[] tasks, CancellationToken cancellationToken, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[], CancellationToken> interval = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], Task> interval = null);
        Task WaitForCompletion(TaskResource[] tasks, CancellationToken cancellationToken, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], CancellationToken, Task> interval = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, TimeSpan? timeoutAfter = null, Func<TaskResource[], Task> interval = null);
        Task WaitForCompletion(TaskResource[] tasks, CancellationToken cancellationToken, int pollIntervalSeconds = 4, TimeSpan? timeoutAfter = null, Func<TaskResource[], CancellationToken, Task> interval = null);
    }

    class TaskRepository : MixedScopeBaseRepository<TaskResource>, ITaskRepository
    {
        public TaskRepository(IOctopusAsyncRepository repository)
            : base(repository, "Tasks")
        {
        }

        TaskRepository(IOctopusAsyncRepository repository, SpaceContext spaceContext)
            : base(repository, "Tasks", spaceContext)
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TaskResource> ExecuteHealthCheck(
            string description = null, 
            int timeoutAfterMinutes = 5, 
            int machineTimeoutAfterMinutes = 1,
            string environmentId = null, 
            string[] machineIds = null,
            string restrictTo = null, 
            string workerpoolId = null, 
            string[] workerIds = null) 
            => ExecuteHealthCheck(CancellationToken.None, description, 
            timeoutAfterMinutes, machineTimeoutAfterMinutes, environmentId, machineIds, 
            restrictTo, workerpoolId, workerIds);

        public Task<TaskResource> ExecuteHealthCheck(CancellationToken cancellationToken, 
            string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null,
            string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
        {
            // Default space enabled -> Creates it in the default space
            // Default space disabled -> Fails
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
            return Create(resource, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TaskResource> ExecuteCalamariUpdate(string description = null, string[] machineIds = null)
            => ExecuteCalamariUpdate(CancellationToken.None, description, machineIds);
        
        public Task<TaskResource> ExecuteCalamariUpdate(CancellationToken cancellationToken, string description = null, string[] machineIds = null)
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
            return Create(resource, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TaskResource> ExecuteBackup(string description = null)
            => ExecuteBackup(CancellationToken.None, description);
        
        public Task<TaskResource> ExecuteBackup(CancellationToken cancellationToken, string description = null)
        {
            EnsureSystemContext();
            var resource = new TaskResource
            {
                Name = BuiltInTasks.Backup.Name,
                Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description
            };
            return CreateSystemTask(resource, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskResource> ExecuteTentacleUpgrade(string description = null, string environmentId = null,
            string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
            => await ExecuteTentacleUpgrade(CancellationToken.None, description, environmentId, machineIds, restrictTo, workerpoolId, workerIds);
        
        public async Task<TaskResource> ExecuteTentacleUpgrade(CancellationToken cancellationToken, string description = null, string environmentId = null, string[] machineIds = null, string restrictTo = null, string workerpoolId = null, string[] workerIds = null)
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
            return await Create(resource, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskResource> ExecuteAdHocScript(string scriptBody, string[] machineIds = null,
            string[] environmentIds = null, string[] targetRoles = null, string description = null,
            string syntax = "PowerShell", BuiltInTasks.AdHocScript.TargetType? targetType = null)
            => await ExecuteAdHocScript(scriptBody, CancellationToken.None, machineIds, environmentIds, targetRoles, description, syntax, targetType);
        
        public async Task<TaskResource> ExecuteAdHocScript(string scriptBody, CancellationToken cancellationToken, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell", BuiltInTasks.AdHocScript.TargetType? targetType = null)
        {
            EnsureSingleSpaceContext();
            await EnsureValidTargetType(targetType, cancellationToken);
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
            return await Create(resource, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskResource> ExecuteActionTemplate(
            ActionTemplateResource template,
            Dictionary<string, PropertyValueResource> properties,
            string[] machineIds = null,
            string[] environmentIds = null,
            string[] targetRoles = null,
            string description = null,
            BuiltInTasks.AdHocScript.TargetType? targetType = null)
            => await ExecuteActionTemplate(template, properties, CancellationToken.None, machineIds, environmentIds, targetRoles, description, targetType);
        
        public async Task<TaskResource> ExecuteActionTemplate(
            ActionTemplateResource template,
            Dictionary<string, PropertyValueResource> properties,
            CancellationToken cancellationToken,
            string[] machineIds = null,
            string[] environmentIds = null,
            string[] targetRoles = null,
            string description = null,
            BuiltInTasks.AdHocScript.TargetType? targetType = null)
        {
            if (string.IsNullOrEmpty(template?.Id)) throw new ArgumentException("The step template was either null, or has no ID");
            await EnsureValidTargetType(targetType, cancellationToken);
            
            var resource = new TaskResource(){SpaceId = template.SpaceId};
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
            return await Create(resource, cancellationToken);
        }

        private async Task EnsureValidTargetType(
            BuiltInTasks.AdHocScript.TargetType? targetType,
            CancellationToken cancellationToken)
        {
            if (targetType == BuiltInTasks.AdHocScript.TargetType.OctopusServer)
            {
                var minimumRequiredVersion = new SemanticVersion("2019.13.5");
                await EnsureServerIsMinimumVersion(minimumRequiredVersion,
                    currentServerVersion => $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with the TargetType value of 'OctopusServer' for this API call. Please upgrade your Octopus Server to version '{minimumRequiredVersion}' or greater.",
                    cancellationToken);
            }
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TaskResource> ExecuteCommunityActionTemplatesSynchronisation(string description = null)
            => ExecuteCommunityActionTemplatesSynchronisation(CancellationToken.None, description);

        public Task<TaskResource> ExecuteCommunityActionTemplatesSynchronisation(CancellationToken cancellationToken,string description = null)
        {
            EnsureSystemContext();

            // SpaceId always need to be null, use a different Create method to handle that
            var resource = new TaskResource
            {
                Name = BuiltInTasks.SyncCommunityActionTemplates.Name,
                Description = description ?? "Run " + BuiltInTasks.SyncCommunityActionTemplates.Name
            };

            return CreateSystemTask(resource, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskDetailsResource> GetDetails(TaskResource resource, bool? includeVerboseOutput = null, int? tail = null)
            => await GetDetails(resource, CancellationToken.None, includeVerboseOutput, tail);
        
        public async Task<TaskDetailsResource> GetDetails(TaskResource resource, CancellationToken cancellationToken, bool? includeVerboseOutput = null, int? tail = null)
        {
            var args = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (includeVerboseOutput.HasValue)
                args.Add("verbose", includeVerboseOutput.Value);

            if (tail.HasValue)
                args.Add("tail", tail.Value);
            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), args);
            return await Client.Get<TaskDetailsResource>(resource.Link("Details"), parameters, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<string> GetRawOutputLog(TaskResource resource)
            => await GetRawOutputLog(resource, CancellationToken.None);

        public async Task<string> GetRawOutputLog(TaskResource resource, CancellationToken cancellationToken)
        {
            return await Client.Get<string>(resource.Link("Raw"), GetAdditionalQueryParameters(), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskTypeResource[]> GetTaskTypes()
            => await GetTaskTypes(CancellationToken.None);
        
        public async Task<TaskTypeResource[]> GetTaskTypes(CancellationToken cancellationToken)
        {
            return await Client.Get<TaskTypeResource[]>((await Client.Repository.LoadRootDocument(cancellationToken)).Links["TaskTypes"], cancellationToken);
        }

        public async Task Prioritize(TaskResource resource, CancellationToken cancellationToken)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            await Client.Post(resource.Link("Prioritize"), (TaskResource)null, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task Rerun(TaskResource resource)
            => await Rerun(resource, CancellationToken.None);
        
        public async Task Rerun(TaskResource resource, CancellationToken cancellationToken)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            await Client.Post(resource.Link("Rerun"), (TaskResource)null, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task Cancel(TaskResource resource)
            => await Cancel(resource, CancellationToken.None);
        
        public async Task Cancel(TaskResource resource, CancellationToken cancellationToken)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            await Client.Post(resource.Link("Cancel"), (TaskResource)null, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task ModifyState(TaskResource resource, TaskState newState, string reason)
            => await ModifyState(resource, newState, reason, CancellationToken.None);
        
        public async Task ModifyState(TaskResource resource, TaskState newState, string reason, CancellationToken cancellationToken)
        {
            EnsureTaskCanRunInTheCurrentContext(resource);
            await Client.Post(resource.Link("State"), new { state = newState, reason = reason }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<IReadOnlyList<TaskResource>> GetQueuedBehindTasks(TaskResource resource)
            => await GetQueuedBehindTasks(resource, CancellationToken.None);
        
        public async Task<IReadOnlyList<TaskResource>> GetQueuedBehindTasks(TaskResource resource, CancellationToken cancellationToken)
        {
            return await Client.ListAll<TaskResource>(resource.Link("QueuedBehind"), GetAdditionalQueryParameters(), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            => WaitForCompletion(task, CancellationToken.None, pollIntervalSeconds, timeoutAfterMinutes, (resources, _) => interval?.Invoke(resources));
        
        public Task WaitForCompletion(TaskResource task, CancellationToken cancellationToken, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[], CancellationToken> interval = null)
        {
            return WaitForCompletion(new[] { task }, cancellationToken, pollIntervalSeconds, timeoutAfterMinutes, interval);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            => WaitForCompletion(tasks, CancellationToken.None, pollIntervalSeconds, timeoutAfterMinutes, (resources, token) => interval?.Invoke(resources));
        
        public Task WaitForCompletion(TaskResource[] tasks, CancellationToken cancellationToken, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[], CancellationToken> interval = null)
        {
            Func<TaskResource[], CancellationToken, Task> taskInterval = null;
            if (interval != null)
                taskInterval = (tr, ct) => Task.Run(() => interval(tr, ct), ct);

            return WaitForCompletion(tasks, cancellationToken, pollIntervalSeconds, timeoutAfterMinutes, taskInterval);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], Task> interval = null)
            => WaitForCompletion(tasks, CancellationToken.None, pollIntervalSeconds, TimeSpan.FromMinutes(timeoutAfterMinutes), 
                (resources, _) => interval == null ? Task.CompletedTask : interval(resources));

        public Task WaitForCompletion(TaskResource[] tasks, CancellationToken cancellationToken, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], CancellationToken, Task> interval = null)
            => WaitForCompletion(tasks, cancellationToken, pollIntervalSeconds, TimeSpan.FromMinutes(timeoutAfterMinutes), interval);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4,
            TimeSpan? timeoutAfter = null, Func<TaskResource[], Task> interval = null)
            => await WaitForCompletion(tasks, CancellationToken.None, pollIntervalSeconds, timeoutAfter,
                (resources, _) => interval == null ? Task.CompletedTask : interval(resources));
        
        public async Task WaitForCompletion(TaskResource[] tasks, CancellationToken cancellationToken, int pollIntervalSeconds = 4, TimeSpan? timeoutAfter = null, Func<TaskResource[], CancellationToken, Task> interval = null)
        {
            var start = Stopwatch.StartNew();
            if (tasks == null || tasks.Length == 0)
                return;
            var additionalQueryParameters = GetAdditionalQueryParameters();
            while (true)
            {
                var stillRunning = await Task.WhenAll(
                        tasks.Select(t => Client.Get<TaskResource>(t.Link("Self"), additionalQueryParameters, cancellationToken))
                    )
                    .ConfigureAwait(false);

                if (interval != null)
                    await interval(stillRunning, cancellationToken).ConfigureAwait(false);

                if (stillRunning.All(t => t.IsCompleted))
                    return;

                if (timeoutAfter.HasValue && timeoutAfter > TimeSpan.Zero && start.Elapsed > timeoutAfter)
                {
                    throw new TimeoutException($"One or more tasks did not complete before the timeout was reached. We waited {start.Elapsed:hh\\:mm\\:ss}  for the tasks to complete.");
                }

                await Task.Delay(TimeSpan.FromSeconds(pollIntervalSeconds), cancellationToken).ConfigureAwait(false);
            }
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TaskResource>> GetAllActive(int pageSize = int.MaxValue) =>
            GetAllActive(CancellationToken.None, pageSize);
        
        public Task<List<TaskResource>> GetAllActive(CancellationToken cancellationToken, int pageSize = int.MaxValue) 
            => FindAll(path: null, pathParameters: new { active = true, take = pageSize }, cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskResourceCollection> GetActiveWithSummary(int pageSize = int.MaxValue, int skip = 0)
            => await GetActiveWithSummary(CancellationToken.None, pageSize, skip);

        public async Task<TaskResourceCollection> GetActiveWithSummary(CancellationToken cancellationToken, int pageSize = int.MaxValue, int skip = 0)
            => await Client.Get<TaskResourceCollection>(await ResolveLink(cancellationToken), new {active = true, take = pageSize, skip}, cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public async Task<TaskResourceCollection> GetAllWithSummary(int pageSize = int.MaxValue, int skip = 0)
            => await GetAllWithSummary(CancellationToken.None, pageSize, skip);
        
        public async Task<TaskResourceCollection> GetAllWithSummary(CancellationToken cancellationToken, int pageSize = int.MaxValue, int skip = 0)
            => await Client.Get<TaskResourceCollection>(await ResolveLink(cancellationToken), new {take = pageSize, skip}, cancellationToken);

        public ITaskRepository UsingContext(SpaceContext spaceContext)
        {
            return new TaskRepository(Repository, spaceContext);
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

        async Task<TaskResource> CreateSystemTask(TaskResource task, CancellationToken cancellationToken)
        {
            return await Client.Create(await Repository.Link(CollectionLinkName).ConfigureAwait(false), task, cancellationToken).ConfigureAwait(false);
        }
    }

    public class SpaceScopedOperationOutsideOfCurrentSpaceContextException : Exception
    {
        public SpaceScopedOperationOutsideOfCurrentSpaceContextException(string spaceId, SpaceContext context) 
            : base($"Attempted to perform a space scoped operation within space {spaceId}, but your current space context does not contain that space id. " +
                   $"Current Space Context: {context.ApplySpaceSelection(spaces => string.Join(", ", spaces), () => "all spaces")}")
        {
        }
    }
}
