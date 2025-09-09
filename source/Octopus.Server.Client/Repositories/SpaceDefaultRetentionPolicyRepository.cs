using System;
using Octopus.Client.Model;
using Octopus.Client.Model.SpaceDefaultRetentionPolicies;
using Octopus.Client.Validation;

namespace Octopus.Client.Repositories;

public interface ISpaceDefaultRetentionPolicyRepository
{
    SpaceDefaultRetentionPolicyResource Get(GetDefaultRetentionPolicyByTypeRequest request);

    SpaceDefaultRetentionPolicyResource Modify(ModifyDefaultRetentionPolicyCommand command);
}

public class SpaceDefaultRetentionPolicyRepository(IOctopusClient client) : ISpaceDefaultRetentionPolicyRepository
{
    const string GetApiRoute = "/api/{spaceId}/retentionpolicies?retentionType={retentionType}";
    const string ModifyApiRoute = "/api/{spaceId}/retentionpolicies/{id}";
    
    static readonly SemanticVersion RequiredVersion = new SemanticVersion(2025, 3, 13969);

    public SpaceDefaultRetentionPolicyResource Get(GetDefaultRetentionPolicyByTypeRequest request)
    {
        EnsureServerCompatability();
        
        return client.Get<SpaceDefaultRetentionPolicyResource>(GetApiRoute, request);
    }

    public SpaceDefaultRetentionPolicyResource Modify(ModifyDefaultRetentionPolicyCommand command)
    {
        EnsureServerCompatability();
        
        return client.Update<ModifyDefaultRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(ModifyApiRoute, command, pathParameters: new { command.SpaceId, command.Id });
    }
    
    void EnsureServerCompatability()
    {
        var rootDocument = client.Repository.LoadRootDocument();
        var currentServerVersion = rootDocument.Version;

        if (ServerVersionCheck.IsOlderThanClient(currentServerVersion, RequiredVersion))
        {
            throw new NotSupportedException($"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{RequiredVersion.ToNormalizedString()}'");
        }
    }
}
