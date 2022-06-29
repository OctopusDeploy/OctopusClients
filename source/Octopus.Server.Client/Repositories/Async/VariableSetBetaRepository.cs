using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IVariableSetBetaRepository
    {
        Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null);
        Task<VariableSetPreviewResource> GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null);
    }

    class VariableSetBetaRepository : IVariableSetBetaRepository
    {
        readonly IOctopusAsyncRepository repository;
        readonly IOctopusAsyncClient client;

        public VariableSetBetaRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
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

        public async Task<VariableSetPreviewResource> GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null)
        {
            var project = projectResource.Id;
            return await client.Get<VariableSetPreviewResource>(await repository.Link("VariablePreview"), new { project, channel, tenant, runbook, action, environment, machine, role, gitRef });
        }

        bool ProjectHasVariablesInGit(ProjectResource projectResource)
        {
            return projectResource.PersistenceSettings is GitPersistenceSettingsResource gitSettings && gitSettings.ConversionState.VariablesAreInGit;
        }
    }
}
