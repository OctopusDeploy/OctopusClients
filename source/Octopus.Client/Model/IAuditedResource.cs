using System;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Implemented by resources that are audited.
    /// </summary>
    public interface IAuditedResource
    {
        /// <summary>
        /// Gets or sets the date/time that this resource was last modified.
        /// </summary>
        DateTimeOffset? LastModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last modified this resource.
        /// </summary>
        string LastModifiedBy { get; set; }
    }
}