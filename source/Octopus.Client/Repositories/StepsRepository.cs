using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IStepsRepository : IGet<StepsResource>, IModify<StepsResource>
    {
        ReleaseTemplateResource GetTemplate(StepsResource deploymentSteps, ChannelResource channel);
    }
    
    class StepsRepository : BasicRepository<StepsResource>, IStepsRepository
    {
        public StepsRepository(IOctopusRepository repository)
            : base(repository, "Steps")
        {
        }

        public ReleaseTemplateResource GetTemplate(StepsResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }
}