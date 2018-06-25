using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Octopus doesn't just store information; it actively *does* things. Examples include deployments, checking that
    /// machines are
    /// online, automated database backups, and more. These "tasks" are queued and executed asynchronously, and their
    /// progress and logs
    /// can be monitored using the HTTP API. Some tasks are created automatically; for example, Octopus will automatically
    /// create a task
    /// to check the status of all machines every 5 minutes. Some tasks are created implicitly, such as when a deployment
    /// is created to execute
    /// the actual deployment. And some tasks can be created manually, such as backup tasks and sending test emails.
    /// </summary>
    public class TaskResource : Resource, IHaveSpaceResource
    {
        /// <summary>
        /// Create a new <see cref="TaskResource" />.
        /// </summary>
        public TaskResource()
        {
            Arguments = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets or sets the name of the task to create. This name must be one of the list of possible names documented in the
        /// create API operation documentation.
        /// </summary>
        [WriteableOnCreate]
        [JsonProperty(Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a short, human-understandable description of this task. An example might be "Manual database backup".
        /// This is the
        /// name that will be shown in the task list.
        /// </summary>
        [WriteableOnCreate]
        [JsonProperty(Order = 3)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets any arguments to the task.
        /// </summary>
        [WriteableOnCreate]
        [JsonProperty(Order = 4)]
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// Gets or sets the current state of the task.
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        [JsonProperty(Order = 5)]
        public TaskState State { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the completion status of the task. May be "Timed out", "Queued...", "Executing...",
        /// or the time at
        /// which the task completed for completed tasks.
        /// </summary>
        [JsonProperty(Order = 6)]
        public string Completed { get; set; }

        /// <summary>
        /// Gets or sets the time at which the task was queued.
        /// </summary>
        [JsonProperty(Order = 7)]
        public DateTimeOffset QueueTime { get; set; }

        /// <summary>
        /// Gets or sets the time that can elapse after the QueueTime before the task will timeout if it has not started executing.
        /// </summary>
        [JsonProperty(Order = 8)]
        public DateTimeOffset? QueueTimeExpiry { get; set; }

        /// <summary>
        /// Gets or sets the time at which the task started executing.
        /// </summary>
        [JsonProperty(Order = 9)]
        public DateTimeOffset? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time that the Octopus server last updated the status of this task. For a running task this should
        /// happen
        /// at least every couple of minutes.
        /// </summary>
        [JsonProperty(Order = 10)]
        public DateTimeOffset? LastUpdatedTime { get; set; }

        /// <summary>
        /// Gets or sets the date/time that the task completed. Will be null if the task has not yet completed.
        /// </summary>
        [JsonProperty(Order = 11)]
        public DateTimeOffset? CompletedTime { get; set; }

        /// <summary>
        /// Gets the ID of the Octopus server that created and will control this task.
        /// </summary>
        [JsonProperty(Order = 15)]
        public string ServerNode { get; set; }

        /// <summary>
        /// Gets or sets a string indicating how long the task took to run.
        /// </summary>
        [JsonProperty(Order = 21)]
        public string Duration { get; set; }

        /// <summary>
        /// Gets or sets a short summary of the errors encountered when the task ran (if any).
        /// </summary>
        [JsonProperty(Order = 22)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the Octopus Server is processing this task.
        /// </summary>
        [JsonProperty(Order = 23)]
        public bool HasBeenPickedUpByProcessor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task has completed (that is, not queued, not running, and not paused;
        /// may have finished successfully or failed).
        /// </summary>
        [JsonProperty(Order = 24)]
        public bool IsCompleted
        {
            get { return State.IsCompleted(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the task ran to completion successfully.
        /// </summary>
        [JsonProperty(Order = 31)]
        public bool FinishedSuccessfully
        {
            get { return State == TaskState.Success; }
        }

        /// <summary>
        /// True if the task is waiting for manual intervention.
        /// </summary>
        [JsonProperty(Order = 32)]
        public bool HasPendingInterruptions { get; set; }

        /// <summary>
        /// If true, then the task can be used as the basis for a
        /// new task with the same effect.
        /// </summary>
        [JsonProperty(Order = 33)]
        public bool CanRerun { get; set; }

        /// <summary>
        /// True if any warnings or non-fatal errors were recorded in
        /// the task log during execution.
        /// </summary>
        [JsonProperty(Order = 34)]
        public bool HasWarningsOrErrors { get; set; }

        public string SpaceId { get; set; }
    }
}