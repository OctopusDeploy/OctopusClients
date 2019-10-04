using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookRepository : IFindByName<RunbookResource>, IGet<RunbookResource>, ICreate<RunbookResource>, IModify<RunbookResource>, IDelete<RunbookResource>
    {
        Task<RunbookResource> FindByName(ProjectResource project, string name);
        Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description);
    }

    class RunbookRepository : BasicRepository<RunbookResource>, IRunbookRepository
    {
        public RunbookRepository(IOctopusAsyncRepository repository)
            : base(repository, "Runbooks")
        {
        }

        public Task<RunbookResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Runbooks"));
        }

        public Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            return new RunbookEditor(this, new RunbookProcessRepository(Repository)).CreateOrModify(project, name, description);
        }
    }
}
