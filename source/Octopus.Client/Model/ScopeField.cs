using System;

namespace Octopus.Client.Model
{
    public enum ScopeField
    {
        Project,
        Environment,
        Machine,
        Role,
        TargetRole,
        Action,
        User,
        Private, // Allows inbuilt vars to override user ones
        Channel,
        TenantTag,
        Tenant, 
        ProcessOwner,
    }
}