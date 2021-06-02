using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IVcsRunbookRepository
    {
        Task<VcsRunbookResource> Get(string runbookId);
        Task<VcsRunbookResource> Modify(VcsRunbookResource resource, string commitMessage = null);
        Task<VcsRunbookResource> Create(VcsRunbookResource resource, string commitMessage = null);
        Task Delete(VcsRunbookResource resource, string commitMessage = null);
        Task<ResourceCollection<VcsRunbookResource>> List(string partialName = null, int skip = 0, int? take = null);
    }

    public class VcsRunbookRepository : IVcsRunbookRepository
    {
        private const string RunbookLinkId = "Runbook";
        private readonly IOctopusAsyncRepository repository;
        private readonly VersionControlBranchResource branch;

        public VcsRunbookRepository(IOctopusAsyncRepository repository, VersionControlBranchResource branch)
        {
            this.repository = repository;
            this.branch = branch;
        }

        public Task<VcsRunbookResource> Get(string runbookId)
        {
            var uri = branch.Link(RunbookLinkId);
            return repository.Client.Get<VcsRunbookResource>(uri, new { id = runbookId });
        }

        public Task<VcsRunbookResource> Modify(VcsRunbookResource resource, string commitMessage = null)
        {
            var uri = resource.Link("Self");
            var resourceWithCommit = new CommitResource<VcsRunbookResource>
            {
                CommitMessage = commitMessage,
                Resource = resource
            };
            repository.Client.Put(uri, resourceWithCommit);
            return repository.Client.Get<VcsRunbookResource>(uri);
        }

        public Task<VcsRunbookResource> Create(VcsRunbookResource resource, string commitMessage = null)
        {
            var uri = branch.Link(RunbookLinkId);
            var resourceWithCommit = new CommitResource<VcsRunbookResource>
            {
                CommitMessage = commitMessage,
                Resource = resource
            };
            return repository.Client.Post<CommitResource<VcsRunbookResource>, VcsRunbookResource>(uri, resourceWithCommit);
        }

        public Task Delete(VcsRunbookResource resource, string commitMessage = null)
        {
            var uri = branch.Link(RunbookLinkId);
            var commit = new CommitResource
            {
                CommitMessage = commitMessage
            };
            return repository.Client.Delete(uri, new { id = resource.Id }, commit);
        }

        public Task<ResourceCollection<VcsRunbookResource>> List(string partialName = null, int skip = 0, int? take = null)
        {
            var uri = branch.Link(RunbookLinkId);
            return repository.Client.List<VcsRunbookResource>(uri, new { skip, take, partialName });
        }
    }
}