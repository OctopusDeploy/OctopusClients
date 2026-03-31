using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async;

public interface IPlatformHubPolicyRepository
{
    Task<CompliancePolicyResource> Create(CompliancePolicyResource resource, string commitMessage, CancellationToken cancellationToken);
    Task<GetCompliancePoliciesResponse> Get(GetCompliancePoliciesRequest request, CancellationToken cancellationToken);
    Task<CompliancePolicyResource> GetBySlug(string gitRef, string slug, CancellationToken cancellationToken);
    Task<CompliancePolicyResource> Modify(CompliancePolicyResource resource, string commitMessage, CancellationToken cancellationToken);
    Task<CompliancePolicyVersionResource> Publish(string gitRef, string slug, string version, CancellationToken cancellationToken);
    Task<GetCompliancePolicyVersionsResponse> GetVersions(GetCompliancePolicyVersionsRequest request, CancellationToken cancellationToken);
    Task<CompliancePolicyVersionResource> ActivateVersion(CompliancePolicyVersionResource version, CancellationToken cancellationToken);
    Task<CompliancePolicyVersionResource> DeactivateVersion(CompliancePolicyVersionResource version, CancellationToken cancellationToken);
}

internal class PlatformHubPolicyRepository : IPlatformHubPolicyRepository
{
    private readonly IOctopusAsyncClient client;

    public PlatformHubPolicyRepository(IOctopusAsyncClient client)
    {
        this.client = client;
    }

    public async Task<CompliancePolicyResource> Create(CompliancePolicyResource resource, string commitMessage, CancellationToken cancellationToken)
    {
        var command = new CreateOrModifyCompliancePolicyCommand(resource, commitMessage);
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForCreate(command.GitRef);
        
        return await client
            .Post<CreateOrModifyCompliancePolicyCommand, CompliancePolicyResource>(
                path: path,
                pathParameters: parameters,
                resource: command,
                cancellationToken: cancellationToken
            ).
            ConfigureAwait(false);
    }

    public async Task<GetCompliancePoliciesResponse> Get(GetCompliancePoliciesRequest request, CancellationToken cancellationToken)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForGet(request);
        return await client
            .Get<GetCompliancePoliciesResponse>(
                path: path,
                pathParameters: parameters,
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task<CompliancePolicyResource> GetBySlug(string gitRef, string slug, CancellationToken cancellationToken)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForGetBySlug(gitRef, slug);
        return await client
            .Get<CompliancePolicyResource>(
                path: path,
                pathParameters: parameters,
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task<CompliancePolicyResource> Modify(CompliancePolicyResource resource, string commitMessage, CancellationToken cancellationToken)
    {
        var command = new CreateOrModifyCompliancePolicyCommand(resource, commitMessage);
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForModify(command.GitRef, command.Slug);
        await client
            .Put(
                path: path,
                pathParameters: parameters,
                resource: command,
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);

        return await GetBySlug(command.GitRef, command.Slug, cancellationToken);
    }

    public async Task<CompliancePolicyVersionResource> Publish(string gitRef, string slug, string version, CancellationToken cancellationToken)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForPublish(gitRef, slug);
        return await client
            .Post<object, CompliancePolicyVersionResource>(
                path: path,
                pathParameters: parameters,
                resource: new { version },
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }
    
    public async Task<GetCompliancePolicyVersionsResponse> GetVersions(GetCompliancePolicyVersionsRequest request, CancellationToken cancellationToken)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForGetVersions(request);
        return await client
            .Get<GetCompliancePolicyVersionsResponse>(
                path: path,
                pathParameters: parameters,
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task<CompliancePolicyVersionResource> ActivateVersion(CompliancePolicyVersionResource version, CancellationToken cancellationToken)
    {
        return await ModifyActivationStatus(version, isActive: true, cancellationToken);
    }

    public async Task<CompliancePolicyVersionResource> DeactivateVersion(CompliancePolicyVersionResource version, CancellationToken cancellationToken)
    {
        return await ModifyActivationStatus(version, isActive: false, cancellationToken);
    }

    private async Task<CompliancePolicyVersionResource> ModifyActivationStatus(CompliancePolicyVersionResource version, bool isActive, CancellationToken cancellationToken)
    {
        var (path, parameters) = PlatformHubPolicyRepositoryPathResolver.ForVersionActivationStatus(version);
        return await client
            .Post<object, CompliancePolicyVersionResource>(
                path: path,
                pathParameters: parameters,
                resource: new { isActive },
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }
}
