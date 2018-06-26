using System;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Model.Forms;

namespace Octopus.Client.Model
{
    /// <summary>
    /// An interruption is a request by a process running in the Octopus server for
    /// user action or input.
    /// </summary>
    public class InterruptionResource : Resource, IHaveSpaceResource
    {
        /// <summary>
        /// Gets or sets a title for this interruption.
        /// </summary>
        [JsonProperty(Order = 2)]
        public string Title { get; set; }

        /// <summary>
        /// Gets the time at which the interruption was created.
        /// </summary>
        [JsonProperty(Order = 5)]
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// True if the interruption is waiting for user action; otherwise, false.
        /// </summary>
        [JsonProperty(Order = 10)]
        public bool IsPending { get; set; }

        /// <summary>
        /// Gets the form requesting user input.
        /// </summary>
        [JsonProperty(Order = 15)]
        public Form Form { get; set; }

        /// <summary>
        /// Gets the ids of documents related to this interruption.
        /// </summary>
        [JsonProperty(Order = 20)]
        public ReferenceCollection RelatedDocumentIds { get; set; }

        /// <summary>
        /// Gets the ids of groups that can take responsibility for this interruption.
        /// </summary>
        [JsonProperty(Order = 25)]
        public ReferenceCollection ResponsibleTeamIds { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        [JsonProperty(Order = 30)]
        public string ResponsibleUserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user has permissions to take responsibility for this
        /// interruption.
        /// </summary>
        [JsonProperty(Order = 35)]
        public bool CanTakeResponsibility { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current user has responsibility for this interruption.
        /// </summary>
        [JsonProperty(Order = 40)]
        public bool HasResponsibility { get; set; }

        /// <summary>
        /// Gets or sets the id of the Server Task raising the interruption.
        /// </summary>
        [JsonProperty(Order = 45)]
        public string TaskId { get; set; }

        /// <summary>
        /// Gets or sets the correlation ID of the activity in which the interruption was requested,
        /// if any.
        /// </summary>
        [JsonProperty(Order = 45)]
        public string CorrelationId { get; set; }

        /// <summary>
        /// If this interruption is linked to another it will be 
        /// automatically completed when the other one is. Used to 
        /// handle interruptions in child deployments.
        /// </summary>
        [JsonProperty(Order = 50)]
        public bool IsLinkedToOtherInterruption { get; set; }

        public string SpaceId { get; set; }
    }
}