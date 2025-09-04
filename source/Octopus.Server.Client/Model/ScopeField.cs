using Newtonsoft.Json;
using Octopus.Client.Serialization;
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
        Trigger, // Allows vars coming via triggers to override scoped values
        ParentDeployment, // Allows vars coming via deploy release step to override scoped values
        Private, // Allows inbuilt vars to override user ones
        Channel,
        TenantTag,
        Tenant,
        ProcessOwner,
        ProjectTemplate,
    }
}
