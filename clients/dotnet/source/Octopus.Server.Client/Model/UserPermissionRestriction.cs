using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Describes the scope of a permission granted to a user.
    /// </summary>
    public class UserPermissionRestriction
    {
        /// <summary>
        /// Restrictions on the projects to which the permission applies,
        /// if any.
        /// </summary>
        public ReferenceCollection RestrictedToProjectIds { get; set; }

        /// <summary>
        /// Restrictions on the environments to which the permission applies,
        /// if any.
        /// </summary>
        public ReferenceCollection RestrictedToEnvironmentIds { get; set; }


        /// <summary>
        /// Restrictions on the tenants to which the permission applies,
        /// if any.
        /// </summary>
        public ReferenceCollection RestrictedToTenantIds { get; set; }

        /// <summary>
        /// Restrictions on the project groups to which the permission applies,
        /// if any.
        /// </summary>
        public ReferenceCollection RestrictedToProjectGroupIds { get; set; }
        
        public string SpaceId { get; set; }

        public static IEqualityComparer<UserPermissionRestriction> UserPermissionRestrictionComparer { get; } = new Comparer();

        public override string ToString()
        {
            return "Projects: " + (RestrictedToProjectIds ?? new ReferenceCollection()) + "; " +
                   "Environments: " + (RestrictedToEnvironmentIds ?? new ReferenceCollection()) + "; " +
                   "Tenants: " + (RestrictedToTenantIds ?? new ReferenceCollection()) + "; " +
                   "Project Groups: " + (RestrictedToProjectGroupIds ?? new ReferenceCollection());
        }

        public sealed class Comparer : IEqualityComparer<UserPermissionRestriction>
        {
            public bool Equals(UserPermissionRestriction x, UserPermissionRestriction y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.ToString(), y.ToString());
            }

            public int GetHashCode(UserPermissionRestriction obj)
            {
                unchecked
                {
                    return obj.ToString().GetHashCode();
                }
            }
        }
    }
}