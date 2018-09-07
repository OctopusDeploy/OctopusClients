using System;
using System.ComponentModel;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Permissions are restricted via roles; a role may be restricted in a way that some of the
    /// included permissions are not. So, when permission sets are loaded we only set up restrictions
    /// that are supported by the permission type.
    /// </summary>
    public enum Permission
    {
        None,

        [Description("Perform system-level functions like configuring HTTP web hosting, the public URL, server nodes, maintenance mode, and server diagnostics")] AdministerSystem,

        [Description("Edit project details")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ProjectEdit,

        [Description("View the details of projects")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ProjectView,

        [Description("Create projects")] [SupportsRestriction(PermissionScope.ProjectGroups)] ProjectCreate,

        [Description("Delete projects")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ProjectDelete,

        [Description("View the deployment process and channels associated with a project")] [SupportsRestriction(PermissionScope.Projects)] ProcessView,

        [Description("Edit the deployment process and channels associated with a project")] [SupportsRestriction(PermissionScope.Projects)] ProcessEdit,

        [Description("Edit variables belonging to a project")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments)] VariableEdit,

        [Description("Edit non-environment scoped variables belonging to a project or library variable set")] [SupportsRestriction(PermissionScope.Projects)] VariableEditUnscoped,

        [Description("View variables belonging to a project or library variable set")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments)] VariableView,

        [Description("View non-environment scoped variables belonging to a project or library variable set")] [SupportsRestriction(PermissionScope.Projects)] VariableViewUnscoped,

        [Description("Create a release for a project")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ReleaseCreate,

        [Description("View a release of a project")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ReleaseView,

        [Description("Edit a release of a project")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ReleaseEdit,

        [Description("Delete a release of a project")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Tenants)] ReleaseDelete,

        [Description("Block a release from progressing to the next lifecycle phase")] [SupportsRestriction(PermissionScope.Projects)] DefectReport,

        [Description("Unblock a release so it can progress to the next phase")] [SupportsRestriction(PermissionScope.Projects)] DefectResolve,

        [Description("Deploy releases to target environments")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] DeploymentCreate,

        [Description("Delete deployments")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] DeploymentDelete,

        [Description("View deployments")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] DeploymentView,

        [Description("View environments")] [SupportsRestriction(PermissionScope.Environments)] EnvironmentView,

        [Description("Create environments")] [SupportsRestriction(PermissionScope.Environments)] EnvironmentCreate,

        [Description("Edit environments")] [SupportsRestriction(PermissionScope.Environments)] EnvironmentEdit,

        [Description("Delete environments")] [SupportsRestriction(PermissionScope.Environments)] EnvironmentDelete,

        [Description("Create machines")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] MachineCreate,

        [Description("Edit machines")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] MachineEdit,

        [Description("View machines")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] MachineView,

        [Description("Delete machines")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] MachineDelete,

        [Description("View the artifacts created manually and during deployment")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] ArtifactView,

        [Description("Manually create artifacts")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] ArtifactCreate,

        [Description("Edit the details describing artifacts")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] ArtifactEdit,

        [Description("Delete artifacts")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] ArtifactDelete,

        [Description("View package feeds and the packages in them")] FeedView,

        [Description("View release and deployment events")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] EventView,

        [Description("View library variable sets")] LibraryVariableSetView,

        [Description("Create library variable sets")] LibraryVariableSetCreate,

        [Description("Edit library variable sets")] LibraryVariableSetEdit,

        [Description("Delete library variable sets")] LibraryVariableSetDelete,

        [Description("View project groups")] [SupportsRestriction(PermissionScope.ProjectGroups)] ProjectGroupView,

        [Description("Create project groups")] [SupportsRestriction(PermissionScope.ProjectGroups)] ProjectGroupCreate,

        [Description("Edit project groups")] [SupportsRestriction(PermissionScope.ProjectGroups)] ProjectGroupEdit,

        [Description("Delete project groups")] [SupportsRestriction(PermissionScope.ProjectGroups)] ProjectGroupDelete,

        [Description("Create teams")] TeamCreate,

        [Description("View teams")] TeamView,

        [Description("Edit teams")] TeamEdit,

        [Description("Delete teams")] TeamDelete,

        [Description("View users")] UserView,

        [Description("Invite users to register accounts")] UserInvite,

        [Description("View other user's roles")] UserRoleView,

        [Description("Edit user role definitions")] UserRoleEdit,


        [Description("View summary-level information associated with a task")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants)] TaskView,

        [Description("View detailed information about the execution of a task, including the task log output")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants)] TaskViewLog,

        [Description("Explicitly create (run) server tasks")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants)] TaskCreate,

        [Description("Cancel server tasks")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants)] TaskCancel,

        [Description("Edit server tasks")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants)] TaskEdit,


        [Description("View interruptions generated during deployments")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] InterruptionView,

        [Description("Take responsibility for and submit interruptions generated during deployments")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] InterruptionSubmit,

        [Description("Take responsibility for and submit interruptions generated during deployments when the user is in a designated responsible team")] [SupportsRestriction(PermissionScope.Projects, PermissionScope.Environments, PermissionScope.Tenants, ExplicitTenantScopeRequired = true)] InterruptionViewSubmitResponsible,

        [Description("Push new packages to the built-in package repository")] [SupportsRestriction(PermissionScope.Projects)] BuiltInFeedPush,

        [Description("Replace or delete packages in the built-in package repository")] [SupportsRestriction(PermissionScope.Projects)] BuiltInFeedAdminister,

        [Description("Retrieve the contents of packages in the built-in package repository")] [SupportsRestriction(PermissionScope.Projects)] BuiltInFeedDownload,

        [Description("View step templates")] ActionTemplateView,

        [Description("Create step templates")] ActionTemplateCreate,

        [Description("Edit step templates")] ActionTemplateEdit,

        [Description("Delete step templates")] ActionTemplateDelete,

        [Description("Create lifecycles")] LifecycleCreate,

        [Description("View lifecycles")] LifecycleView,

        [Description("Edit lifecycles")] LifecycleEdit,

        [Description("Delete lifecycles")] LifecycleDelete,

        [Description("View accounts")] [SupportsRestriction(PermissionScope.Environments)] AccountView,

        [Description("Edit accounts")] [SupportsRestriction(PermissionScope.Environments)] AccountEdit,

        [Description("Create accounts")] [SupportsRestriction(PermissionScope.Environments)] AccountCreate,

        [Description("Delete accounts")] [SupportsRestriction(PermissionScope.Environments)] AccountDelete,

        [Description("Create tenants")] [SupportsRestriction(PermissionScope.Tenants)] TenantCreate,

        [Description("Edit tenants")] [SupportsRestriction(PermissionScope.Tenants)] TenantEdit,

        [Description("View tenants")] [SupportsRestriction(PermissionScope.Tenants)] TenantView,

        [Description("Delete tenants")] [SupportsRestriction(PermissionScope.Tenants)] TenantDelete,

        [Description("Create tag sets")] TagSetCreate,

        [Description("Edit tag sets")] TagSetEdit,

        [Description("Delete tag sets")] TagSetDelete,

        [Description("Create health check policies")] MachinePolicyCreate,

        [Description("View health check policies")] MachinePolicyView,

        [Description("Edit health check policies")] MachinePolicyEdit,

        [Description("Delete health check policies")] MachinePolicyDelete,

        [Description("Create proxies")] ProxyCreate,

        [Description("View proxies")] ProxyView,

        [Description("Edit proxies")] ProxyEdit,

        [Description("Delete proxies")] ProxyDelete,

        [Description("Create subscriptions")] SubscriptionCreate,

        [Description("View subscriptions")] SubscriptionView,

        [Description("Edit subscriptions")] SubscriptionEdit,

        [Description("Delete subscriptions")] SubscriptionDelete,

        [Description("Create triggers")] [SupportsRestriction(PermissionScope.Projects)] TriggerCreate,

        [Description("View triggers")] [SupportsRestriction(PermissionScope.Projects)] TriggerView,

        [Description("Edit triggers")] [SupportsRestriction(PermissionScope.Projects)] TriggerEdit,

        [Description("Delete triggers")] [SupportsRestriction(PermissionScope.Projects)] TriggerDelete,

        [Description("View certificates")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] CertificateView,

        [Description("Create certificates")] CertificateCreate,

        [Description("Edit certificates")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] CertificateEdit,

        [Description("Delete certificates")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] CertificateDelete,

        [Description("Export certificate private-keys")] [SupportsRestriction(PermissionScope.Environments, PermissionScope.Tenants)] CertificateExportPrivateKey,

        [Description("Edit users")] UserEdit,
        
        [Description("Configure server settings like Authentication, SMTP, and HTTP Security Headers")] ConfigureServer,

        [Description("Edit feeds")] FeedEdit,

        [Description("View the workers in worker pools")] WorkerView,

        [Description("Edit workers and worker pools")] WorkerEdit,

        [Description("Run background actions that don't require user principal")] RunSystem,
        
        [Description("Edit spaces")] SpaceEdit,
        
        [Description("View spaces")] SpaceView,
        
        [Description("Delete spaces")] SpaceDelete,
        
        [Description("Create spaces")] SpaceCreate
    }
}