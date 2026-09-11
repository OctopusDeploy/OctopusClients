using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.GitCredentials;

namespace Octopus.Client.Repositories.Async
{
    public interface IGitCredentialRepository : IGet<GitCredentialResource>, IFindByName<GitCredentialResource>, ICreate<GitCredentialResource>, IModify<GitCredentialResource>, IDelete<GitCredentialResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<GitCredentialUsage> Usage(GitCredentialResource credential);
        Task<GitCredentialUsage> Usage(GitCredentialResource credential, CancellationToken cancellationToken);
    }

    class GitCredentialRepository : BasicRepository<GitCredentialResource>, IGitCredentialRepository
    {
        public GitCredentialRepository(IOctopusAsyncRepository repository) : base(repository, "GitCredentials")
        {
        }

        public Task<GitCredentialUsage> Usage(GitCredentialResource credential)
        {
            return Usage(credential, CancellationToken.None);
        }

        public Task<GitCredentialUsage> Usage(GitCredentialResource credential, CancellationToken cancellationToken)
        {
            return Client.Get<GitCredentialUsage>(credential.Link("Usage"), cancellationToken);
        }
    }
}
