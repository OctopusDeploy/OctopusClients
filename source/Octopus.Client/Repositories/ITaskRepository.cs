using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ITaskRepository : IPaginate<TaskResource>, IGet<TaskResource>, ICreate<TaskResource>
    {
        TaskResource ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null);
        TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null);
        TaskResource ExecuteBackup(string description = null);
        TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null);
        TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell");
        TaskResource ExecuteActionTemplate(ActionTemplateResource resource, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null);
        TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null);
        TaskDetailsResource GetDetails(TaskResource resource);
        string GetRawOutputLog(TaskResource resource);
        void Rerun(TaskResource resource);
        void Cancel(TaskResource resource);
        void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
        void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null);
    }
}