using System.Threading.Tasks;
using Octopus.Client.Model.GitCredentials;

namespace Octopus.Client.Repositories.Async
{
    public interface IGitCredentialRepository : IGet<GitCredentialResource>, IFindByName<GitCredentialResource>, ICreate<GitCredentialResource>, IModify<GitCredentialResource>, IDelete<GitCredentialResource>
    {
        Task<GitCredentialUsage> Usage(GitCredentialResource credential);
    }

    class GitCredentialRepository : BasicRepository<GitCredentialResource>, IGitCredentialRepository
    {
        public GitCredentialRepository(IOctopusAsyncRepository repository) : base(repository, "GitCredentials")
        {
        }

        public Task<GitCredentialUsage> Usage(GitCredentialResource credential)
        {
            return Client.Get<GitCredentialUsage>(credential.Link("Usage"));
        }
    }
}