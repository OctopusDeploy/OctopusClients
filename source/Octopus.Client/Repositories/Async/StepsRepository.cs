using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IStepsRepository : IGet<StepsResource>, IModify<StepsResource>
    {
        Task<ReleaseTemplateResource> GetTemplate(StepsResource deploymentProcess, ChannelResource channel);
    }

    class StepsRepository : BasicRepository<StepsResource>, IStepsRepository
    {
        public StepsRepository(IOctopusAsyncRepository repository)
            : base(repository, "Steps")
        {
        }

        public Task<ReleaseTemplateResource> GetTemplate(StepsResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }
}
