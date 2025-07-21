using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// A role played by users.
    /// </summary>
    public class UserRoleResource : Resource, INamedResource
    {
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        public List<string> SupportedRestrictions { get; set; }
        public List<string> SpacePermissionDescriptions { get; set; }
        public List<string> SystemPermissionDescriptions { get; set; }

        [Writeable]
        public List<Permission> GrantedSpacePermissions { get; set; }

        [Writeable]
        public List<Permission> GrantedSystemPermissions { get; set; }

        public bool CanBeDeleted { get; set; }
    }
}