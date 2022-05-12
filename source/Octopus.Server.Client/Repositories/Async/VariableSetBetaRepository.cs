using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IVariableSetBetaRepository
    {
        Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null);
    }

    class VariableSetBetaRepository : IVariableSetBetaRepository
    {
        readonly IOctopusAsyncClient client;

        public VariableSetBetaRepository(IOctopusAsyncRepository repository)
        {
            client = repository.Client;
        }

        public async Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null)
        {
            if (ProjectHasVariablesInGit(projectResource))
            {
                if (gitRef is not null)
                {
                    return await client.Get<VariableSetResource>(projectResource.Link("Variables"), new { gitRef });
                }

                return await client.Get<VariableSetResource>(projectResource.Link("SensitiveVariables"));
            }

            return await client.Get<VariableSetResource>(projectResource.Link("Variables"));
        }

        bool ProjectHasVariablesInGit(ProjectResource projectResource)
        {
            return projectResource.PersistenceSettings is GitPersistenceSettingsResource gitSettings && gitSettings.ConversionState.VariablesAreInGit;
        }
    }
}
