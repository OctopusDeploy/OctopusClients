using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.SpaceDefaultRetentionPolicies;
using Octopus.Client.Validation;

namespace Octopus.Client.Repositories.Async
{
    public interface ISpaceDefaultRetentionPolicyRepository
    {
        Task<SpaceDefaultRetentionPolicyResource> Get(GetDefaultRetentionPolicyByTypeRequest request, CancellationToken cancellationToken);

        Task<SpaceDefaultRetentionPolicyResource> Modify(ModifyDefaultRetentionPolicyCommand command, CancellationToken cancellationToken);
    }

    public class SpaceDefaultRetentionPolicyRepository(IOctopusAsyncClient client) : ISpaceDefaultRetentionPolicyRepository
    {
        const string GetApiRoute = "/api/{spaceId}/retentionpolicies?retentionType={retentionType}";
        const string ModifyApiRoute = "/api/{spaceId}/retentionpolicies/{id}";

        static readonly SemanticVersion RequiredVersion = new SemanticVersion(2025, 3, 13969);

        public async Task<SpaceDefaultRetentionPolicyResource> Get(GetDefaultRetentionPolicyByTypeRequest request, CancellationToken cancellationToken)
        {
            await EnsureServerCompatability(cancellationToken);
            
            var response = await client.Get<SpaceDefaultRetentionPolicyResource>(
                GetApiRoute, request, cancellationToken);

            return response;
        }
        
        public async Task<SpaceDefaultRetentionPolicyResource> Modify(ModifyDefaultRetentionPolicyCommand command, CancellationToken cancellationToken)
        {
            await EnsureServerCompatability(cancellationToken);
            
            var response = await client.Update<ModifyDefaultRetentionPolicyCommand, SpaceDefaultRetentionPolicyResource>(
                ModifyApiRoute, command, pathParameters: new { command.SpaceId, command.Id }, cancellationToken);

            return response;
        }

        async Task EnsureServerCompatability(CancellationToken cancellationToken)
        {
            var rootDocument = await client.Repository.LoadRootDocument(cancellationToken);
            var currentServerVersion = rootDocument.Version;

            if (ServerVersionCheck.IsOlderThanClient(currentServerVersion, RequiredVersion))
            {
                throw new NotSupportedException($"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{RequiredVersion.ToNormalizedString()}'");
            }
        }
    }
}
