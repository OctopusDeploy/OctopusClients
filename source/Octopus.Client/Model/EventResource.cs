using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Events are automatically created when significant actions take place within Octopus by users. Examples are adding
    /// environments, modifying projects,
    /// deploying releases, canceling tasks, and so on. Events can be used to provide an audit trail of what has happened
    /// in the system. The HTTP API *cannot*
    /// be used to add, modify or delete events.
    /// </summary>
    public class EventResource : Resource, IHaveSpaceResource
    {
        /// <summary>
        /// Gets or sets a collection of document ID's that this event relates to. Note that the document ID's may no longer
        /// exist.
        /// </summary>
        public ReferenceCollection RelatedDocumentIds { get; set; }

        /// <summary>
        /// Gets or sets the event category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created the event.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who created the event.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets a description of how the user performing the event
        /// identified themselves to Octopus.
        /// </summary>
        public string IdentityEstablishedWith { get; set; }

        /// <summary>
        /// Gets or sets the date/time that the event took place.
        /// </summary>
        public DateTimeOffset Occurred { get; set; }

        /// <summary>
        /// Gets or sets the message text that summarizes the event.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the message text that summarizes the event, HTML formatted with links to the related documents.
        /// </summary>
        public string MessageHtml { get; set; }

        /// <summary>
        /// Gets or sets an array of document ID's and indexes where they are mentioned in the message text.
        /// </summary>
        public List<EventReference> MessageReferences { get; set; }

        /// <summary>
        /// Gets or sets any user-provided comments that were recorded with the event.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the details of the event. For events representing a modification to a document this will provide a
        /// HTML-formatted diff of the original and new document.
        /// </summary>
        public string Details { get; set; }

        public string SpaceId { get; set; }
    }
}