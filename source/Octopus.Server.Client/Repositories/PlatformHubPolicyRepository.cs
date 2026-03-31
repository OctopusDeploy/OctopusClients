using Octopus.Client.Model;

namespace Octopus.Client.Repositories;

public interface IPlatformHubPolicyRepository
{
    CompliancePolicyResource Create(CompliancePolicyResource resource, string commitMessage);
    GetCompliancePoliciesResponse Get(GetCompliancePoliciesRequest request);
    CompliancePolicyResource GetBySlug(string gitRef, string slug);
    CompliancePolicyResource Modify(CompliancePolicyResource resource, string commitMessage);
    CompliancePolicyVersionResource Publish(string gitRef, string slug, string version);
    GetCompliancePolicyVersionsResponse GetVersions(GetCompliancePolicyVersionsRequest request);
    CompliancePolicyVersionResource ActivateVersion(CompliancePolicyVersionResource version);
    CompliancePolicyVersionResource DeactivateVersion(CompliancePolicyVersionResource version);
}

internal class PlatformHubPolicyRepository : IPlatformHubPolicyRepository
{
    private readonly IOctopusClient client;

    public PlatformHubPolicyRepository(IOctopusClient client)
    {
        this.client = client;
    }

    public CompliancePolicyResource Create(CompliancePolicyResource resource, string commitMessage)
    {
        var command = new CreateOrModifyCompliancePolicyCommand(resource, commitMessage);
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForCreate(command.GitRef);
        
        return client.Post<CreateOrModifyCompliancePolicyCommand, CompliancePolicyResource>(
            path: path,
            pathParameters: parameters,
            resource: command
        );
    }

    public GetCompliancePoliciesResponse Get(GetCompliancePoliciesRequest request)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForGet(request);
        return client.Get<GetCompliancePoliciesResponse>(
            path: path,
            pathParameters: parameters
        );
    }

    public CompliancePolicyResource GetBySlug(string gitRef, string slug)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForGetBySlug(gitRef, slug);
        return client.Get<CompliancePolicyResource>(
            path: path,
            pathParameters: parameters
        );
    }

    public CompliancePolicyResource Modify(CompliancePolicyResource resource, string commitMessage)
    {
        var command = new CreateOrModifyCompliancePolicyCommand(resource, commitMessage);
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForModify(command.GitRef, command.Slug);
        client.Put(
            path: path,
            pathParameters: parameters,
            resource: command
        );

        return GetBySlug(command.GitRef, command.Slug);
    }

    public CompliancePolicyVersionResource Publish(string gitRef, string slug, string version)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForPublish(gitRef, slug);
        return client.Post<object, CompliancePolicyVersionResource>(
            path: path,
            pathParameters: parameters,
            resource: new { version }
        );
    }

    public GetCompliancePolicyVersionsResponse GetVersions(GetCompliancePolicyVersionsRequest request)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForGetVersions(request);
        
        return client.Get<GetCompliancePolicyVersionsResponse>(
            path: path, 
            pathParameters: parameters
        );
    }

    public CompliancePolicyVersionResource ActivateVersion(CompliancePolicyVersionResource version)
    {
        return ModifyActivationStatus(version, isActive: true);
    }

    public CompliancePolicyVersionResource DeactivateVersion(CompliancePolicyVersionResource version)
    {
        return ModifyActivationStatus(version, isActive: false);
    }

    private CompliancePolicyVersionResource ModifyActivationStatus(CompliancePolicyVersionResource version, bool isActive)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForVersionActivationStatus(version);
        return client.Post<object, CompliancePolicyVersionResource>(
            path: path,
            pathParameters: parameters,
            resource: new { isActive }
        );
    }
}

internal class CreateOrModifyCompliancePolicyCommand
{
    public string GitRef { get; }
    public string Slug { get; }
    public string Name { get; }
    public string Description { get; }
    public string ScopeRego { get; }
    public string ConditionsRego { get; }
    public string ViolationReason { get; }
    public string ViolationAction { get; }
    public string ChangeDescription { get; }

    public CreateOrModifyCompliancePolicyCommand(CompliancePolicyResource resource, string commitMessage)
    {
        GitRef = resource.GitRef;
        Slug = resource.Slug;
        Name = resource.Name;
        Description = resource.Description;
        ScopeRego = resource.ScopeRego;
        ConditionsRego = resource.ConditionsRego;
        ViolationReason = resource.ViolationReason;
        ViolationAction = resource.ViolationAction;
        ChangeDescription = commitMessage;
    }
}

internal static class PlatformHubPolicyRepositoryPathResolver
{
    public static (string, object) ForCreate(string gitRef)
    {
       return ("~/api/platformhub/{gitRef}/policies", new { gitRef });  
    }

    public static (string, object) ForGet(GetCompliancePoliciesRequest request)
    {
        return (
            "~/api/platformhub/{gitRef}/policies?partialName={partialName}&skip={skip}&take={take}",
            request
        );
    }
    
    public static (string, object) ForGetBySlug(string gitRef, string slug)
    {
        return (
            "~/api/platformhub/{gitRef}/policies/{slug}",
            new { gitRef, slug }
        );
    }
    
    public static (string, object) ForModify(string gitRef, string slug)
    {
        return (
            "~/api/platformhub/{gitRef}/policies/{slug}",
            new { gitRef, slug }
        );
    }
    
    public static (string, object) ForPublish(string gitRef, string slug)
    {
        return (
            "~/api/platformhub/{gitRef}/policies/{slug}/publish",
            new { gitRef, slug }
        );
    }
    
    public static (string, object) ForGetVersions(GetCompliancePolicyVersionsRequest request)
    {
        return (
            "~/api/platformhub/policies/{slug}/versions/v2?skip={skip}&take={take}",
            request
        );
    }
    
    public static (string, object) ForVersionActivationStatus(CompliancePolicyVersionResource version)
    {
        return (
            "~/api/platformhub/policies/{slug}/versions/{version}/modify-status",
            new { version.Slug, version.Version }
        );
    }
} 
