using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Summarizes the permissions assigned to a user via their
    /// team membership.
    /// </summary>
    public class UserPermissionSetResource : Resource
    {
        /// <summary>
        /// Lists individual space permissions granted, including restrictions where
        /// applicable.
        /// </summary>
        /// <remarks>
        /// Multiple entries may exist for any permission if different
        /// restrictions are applied. Duplicate or redundant entries may be
        /// excluded.
        /// </remarks>
        public Dictionary<Permission, List<UserPermissionRestriction>> SpacePermissions { get; set; }

        /// <summary>
        /// Lists individual system permissions granted, these do not have restrictions
        /// </summary>
        public List<Permission> SystemPermissions { get; set; }

        /// <summary>
        /// Gets the teams that the user is a member of.
        /// </summary>
        public List<ProjectedTeamReferenceDataItem> Teams { get; set; }
    }
}