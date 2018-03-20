using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TeamV2Resource : Resource, INamedResource
    {
        public TeamV2Resource()
        {
            ExternalSecurityGroups = new NamedReferenceItemCollection();
            RoleScopes = new List<RoleScope>();
        }

        /// <summary>
        /// Gets or sets the name of this team.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        /// <summary>
        /// The users who belong to the team.
        /// </summary>
        [Writeable]
        public ReferenceCollection MemberUserIds { get; set; }

        /// <summary>
        /// The externally-managed security groups (e.g., Active Directory groups) who belong to the team.
        /// </summary>
        [Writeable]
        public NamedReferenceItemCollection ExternalSecurityGroups { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the team can be deleted. The built-in teams
        /// provided by Octopus generally cannot be deleted.
        /// </summary>
        public bool CanBeDeleted { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the team can be renamed. The built-in teams
        /// provided by Octopus generally cannot be renamed.
        /// </summary>
        public bool CanBeRenamed { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the team's roles can be changed. The built-in Octopus Administrators team
        /// provided by Octopus cannot have its roles modified; all other teams can.
        /// </summary>
        public bool CanChangeRoles { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the members of this team can be changed. The built-in Everyone team
        /// provided by Octopus cannot have its members changed, as it will always contain all users.
        /// </summary>
        public bool CanChangeMembers { get; set; }

        /// <summary>
        /// The set of roles and scopes that this team will have
        /// </summary>
        public List<RoleScope> RoleScopes { get; set; }
    }

    public class RoleScope
    {
        /// <summary>
        /// The role for which scoping will apply
        /// </summary>
        [Writeable]
        public string UserRoleId { get; set; }

        /// <summary>
        /// The project groups that the team can exercise its roles for. Includes all projects in the groups.
        /// </summary>
        [Writeable]
        public ReferenceCollection ProjectGroupIds { get; set; }

        /// <summary>
        /// The projects that the team can exercise its roles in. If empty,
        /// the team can exercise its roles in all projects.
        /// </summary>
        [Writeable]
        public ReferenceCollection ProjectIds { get; set; }

        /// <summary>
        /// The environments that the team can exercise its roles in. If empty,
        /// the team can exercise its roles in all environments.
        /// </summary>
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        /// <summary>
        /// The tenants that the team can exercise its roles for. If empty,
        /// the team can exercise its roles for all tenants.
        /// </summary>
        [Writeable]
        public ReferenceCollection TenantIds { get; set; }
    }

    /// <summary>
    /// A group of users that can be assigned to roles in projects and environments.
    /// </summary>
    public class TeamResource : Resource, INamedResource
    {
        public TeamResource()
        {
            ExternalSecurityGroups = new NamedReferenceItemCollection();
            TenantTags = new ReferenceCollection();
        }

        /// <summary>
        /// Gets or sets the name of this team.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        /// <summary>
        /// The users who belong to the team.
        /// </summary>
        [Writeable]
        public ReferenceCollection MemberUserIds { get; set; }

        /// <summary>
        /// The externally-managed security groups (e.g., Active Directory groups) who belong to the team.
        /// </summary>
        [Writeable]
        public NamedReferenceItemCollection ExternalSecurityGroups { get; set; }

        /// <summary>
        /// The roles that the team belongs to.
        /// </summary>
        [Writeable]
        public ReferenceCollection UserRoleIds { get; set; }

        /// <summary>
        /// The project groups that the team can exercise its roles for. Includes all projects in the groups.
        /// </summary>
        [Writeable]
        public ReferenceCollection ProjectGroupIds { get; set; }
        
        /// <summary>
        /// The projects that the team can exercise its roles in. If empty,
        /// the team can exercise its roles in all projects.
        /// </summary>
        [Writeable]
        public ReferenceCollection ProjectIds { get; set; }

        /// <summary>
        /// The environments that the team can exercise its roles in. If empty,
        /// the team can exercise its roles in all environments.
        /// </summary>
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        /// <summary>
        /// The tenants that the team can exercise its roles for. If empty,
        /// the team can exercise its roles for all tenants.
        /// </summary>
        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        /// <summary>
        /// Tags that are evaluated on demand to act as if the tenant was explicitly selected. 
        /// </summary>
        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the team can be deleted. The built-in teams
        /// provided by Octopus generally cannot be deleted.
        /// </summary>
        public bool CanBeDeleted { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the team can be renamed. The built-in teams
        /// provided by Octopus generally cannot be renamed.
        /// </summary>
        public bool CanBeRenamed { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the team's roles can be changed. The built-in Octopus Administrators team
        /// provided by Octopus cannot have its roles modified; all other teams can.
        /// </summary>
        public bool CanChangeRoles { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the members of this team can be changed. The built-in Everyone team
        /// provided by Octopus cannot have its members changed, as it will always contain all users.
        /// </summary>
        public bool CanChangeMembers { get; set; }
    }
}
