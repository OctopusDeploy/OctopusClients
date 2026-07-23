using Octopus.Client.Model.RateLimiting;

namespace Octopus.Client.Repositories;

public interface IRateLimitingPolicyRepository
{
    ListRateLimitingPoliciesResponse List(ListRateLimitingPoliciesRequest request);
    RateLimitingPolicyResource Get(GetRateLimitingPolicyByIdRequest request);
    RateLimitingPolicyResource Modify(ModifyRateLimitingPolicyCommand command);
}

internal class RateLimitingPolicyRepository : IRateLimitingPolicyRepository
{
    private readonly IOctopusClient client;

    public RateLimitingPolicyRepository(IOctopusClient client)
    {
        this.client = client;
    }

    public ListRateLimitingPoliciesResponse List(ListRateLimitingPoliciesRequest request)
    {
        const string path = RateLimitingPolicyRepositoryPaths.List;

        return client.Get<ListRateLimitingPoliciesResponse>(path, request);
    }

    public RateLimitingPolicyResource Get(GetRateLimitingPolicyByIdRequest request)
    {
        const string path = RateLimitingPolicyRepositoryPaths.GetById;

        return client.Get<RateLimitingPolicyResource>(path, request);
    }

    public RateLimitingPolicyResource Modify(ModifyRateLimitingPolicyCommand command)
    {
        const string path = RateLimitingPolicyRepositoryPaths.Modify;

        return client.Update<ModifyRateLimitingPolicyCommand, RateLimitingPolicyResource>(path, command, command);
    }
}
