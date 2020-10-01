using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IVcsRunbookRepository
    {
        VcsRunbookResource Get(string runbookId);
        VcsRunbookResource Modify(VcsRunbookResource resource, string commitMessage = null);
        VcsRunbookResource Create(VcsRunbookResource resource, string commitMessage = null);
        void Delete(VcsRunbookResource resource, string commitMessage = null);
        ResourceCollection<VcsRunbookResource> List(string partialName = null, int skip = 0, int? take = null);
    }

    public class VcsRunbookRepository : IVcsRunbookRepository
    {
        private const string RunbookLinkId = "Runbook";
        private readonly IOctopusRepository repository;
        private readonly VersionControlBranchResource branch;

        public VcsRunbookRepository(IOctopusRepository repository, VersionControlBranchResource branch)
        {
            this.repository = repository;
            this.branch = branch;
        }

        public VcsRunbookResource Get(string runbookId)
        {
            var uri = branch.Link(RunbookLinkId);
            return repository.Client.Get<VcsRunbookResource>(uri, new { id = runbookId });
        }

        public VcsRunbookResource Modify(VcsRunbookResource resource, string commitMessage = null)
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

        public VcsRunbookResource Create(VcsRunbookResource resource, string commitMessage = null)
        {
            var uri = branch.Link(RunbookLinkId);
            var resourceWithCommit = new CommitResource<VcsRunbookResource>
            {
                CommitMessage = commitMessage,
                Resource = resource
            };
            return repository.Client.Post<CommitResource<VcsRunbookResource>, VcsRunbookResource>(uri, resourceWithCommit);
        }

        public void Delete(VcsRunbookResource resource, string commitMessage = null)
        {
            var uri = branch.Link(RunbookLinkId);
            var commit = new CommitResource
            {
                CommitMessage = commitMessage
            };
            repository.Client.Delete(uri, new { id = resource.Id }, commit);
        }

        public ResourceCollection<VcsRunbookResource> List(string partialName = null, int skip = 0, int? take = null)
        {
            var uri = branch.Link(RunbookLinkId);
            return repository.Client.List<VcsRunbookResource>(uri, new { skip, take, partialName });
        }
    }
}