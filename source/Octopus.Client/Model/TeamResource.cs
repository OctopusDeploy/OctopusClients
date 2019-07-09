using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TeamResource : Resource, INamedResource, IHaveSpaceResource
    {
        public TeamResource()
        {
            ExternalSecurityGroups = new NamedReferenceItemCollection();
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

        public string SpaceId { get; set; }
    }
    
    /// <summary>
    /// The set of roles and scopes that this team will have
    /// </summary>
    public class ScopedUserRoleResource : Resource, IHaveSpaceResource
    {
        public ScopedUserRoleResource()
        {
            ProjectIds = new ReferenceCollection();
            EnvironmentIds = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
            ProjectGroupIds = new ReferenceCollection();
        }

        /// <summary>
        /// The role for which scoping will apply
        /// </summary>
        [Writeable]
        public string UserRoleId { get; set; }

        [Writeable]
        public string TeamId { get; set; }

        /// <summary>
        /// The project groups that the team can exercise its roles for. Includes all projects in the groups.
        /// </summary>
        [Writeable]
        public ReferenceCollection ProjectGroupIds { get; set; }
        [Writeable]
        public ScopeResource<ProjectGroupResource> ProjectGroupScope { get; set; }

        /// <summary>
        /// The projects that the team can exercise its roles in. If empty,
        /// the team can exercise its roles in all projects.
        /// </summary>
        [Writeable]
        public ReferenceCollection ProjectIds { get; set; }
        
        [Writeable]
        public ScopeResource<ProjectResource> ProjectScope { get; set; }
        /// <summary>
        /// The environments that the team can exercise its roles in. If empty,
        /// the team can exercise its roles in all environments.
        /// </summary>
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }
        
        [Writeable]
        public ScopeResource<EnvironmentResource> EnvironmentScope { get; set; }
        
        /// <summary>
        /// The tenants that the team can exercise its roles for. If empty,
        /// the team can exercise its roles for all tenants.
        /// </summary>
        [Writeable]
        public ReferenceCollection TenantIds { get; set; }
        
        [Writeable]
        public ScopeResource<TenantResource> TenantScope { get; set; }
        
        public string SpaceId { get; set; }
    }

    /// <summary>
    /// Determines the type of scoping is chosen for a given resource collection.
    /// </summary>
    public enum ChoiceType
    {
        /// <summary>
        /// All resources are available
        /// </summary>
        Unrestricted,
        /// <summary>
        /// Only provide scoped access to a list of resources 
        /// </summary>
        OnlyThese,
        /// <summary>
        /// Provide an exclusion list of resources, provide scoped access to all others
        /// </summary>
        AllExceptThese
    }
    
    public class ScopeResource<TResource> where TResource : IResource {
        /// <summary>
        /// The type of scope query logic to use. <see cref="ChoiceType"/>
        /// </summary>
        public ChoiceType ChoiceType { get; set; }
        /// <summary>
        /// The list of resources to include
        /// </summary>
        public ResourceCollection<TResource> Ids { get; set;  }
        /// <summary>
        /// Include resources that aren't scoped
        /// </summary>
        public bool IncludeUnscoped { get; set; }
    }
}
