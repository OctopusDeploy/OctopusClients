using System;

namespace Octopus.Client.Model
{
    public class DashboardItemResource : Resource
    {
        public string ProjectId { get; set; }
        public string EnvironmentId { get; set; }
        public string ReleaseId { get; set; }
        public string DeploymentId { get; set; }
        public string TaskId { get; set; }
        public string TenantId { get; set; }
        public string ChannelId { get; set; }
        public string ReleaseVersion { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset QueueTime { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? CompletedTime { get; set; }
        public TaskState State { get; set; }
        public bool HasPendingInterruptions { get; set; }
        public bool HasWarningsOrErrors { get; set; }
        public string ErrorMessage { get; set; }
        public string Duration { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsPrevious { get; set; }
        public bool IsCompleted { get; set; }
    }
}