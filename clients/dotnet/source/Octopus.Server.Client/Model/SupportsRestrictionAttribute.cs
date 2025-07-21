using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SupportsRestrictionAttribute : Attribute
    {
        public SupportsRestrictionAttribute(params string[] scopes)
        {
            this.Scopes = (IList<string>) scopes ?? new List<string>();
        }

        public IList<string> Scopes { get; }

        public bool ExplicitTenantScopeRequired { get; set; }
    }
}