using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.RateLimiting;

namespace Octopus.Client.Repositories.Async;

public interface IRateLimitingPolicyRepository
{
    Task<ListRateLimitingPoliciesResponse> List(ListRateLimitingPoliciesRequest request, CancellationToken cancellationToken);
    Task<RateLimitingPolicyResource> Get(GetRateLimitingPolicyByIdRequest request, CancellationToken cancellationToken);
    Task<RateLimitingPolicyResource> Modify(ModifyRateLimitingPolicyCommand command, CancellationToken cancellationToken);
}

internal class RateLimitingPolicyRepository : IRateLimitingPolicyRepository
{
    private readonly IOctopusAsyncClient client;

    public RateLimitingPolicyRepository(IOctopusAsyncClient client)
    {
        this.client = client;
    }

    public async Task<ListRateLimitingPoliciesResponse> List(ListRateLimitingPoliciesRequest request, CancellationToken cancellationToken)
    {
        const string path = RateLimitingPolicyRepositoryPaths.List;

        return await client
            .Get<ListRateLimitingPoliciesResponse>(path, request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<RateLimitingPolicyResource> Get(GetRateLimitingPolicyByIdRequest request, CancellationToken cancellationToken)
    {
        const string path = RateLimitingPolicyRepositoryPaths.GetById;

        return await client
            .Get<RateLimitingPolicyResource>(path, request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<RateLimitingPolicyResource> Modify(ModifyRateLimitingPolicyCommand command, CancellationToken cancellationToken)
    {
        const string path = RateLimitingPolicyRepositoryPaths.Modify;

        return await client
            .Update<ModifyRateLimitingPolicyCommand, RateLimitingPolicyResource>(path, command, command, cancellationToken)
            .ConfigureAwait(false);
    }
}
