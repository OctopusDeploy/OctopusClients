using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Git;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectBetaRepository
    {
        Task<ConvertProjectVariablesToGitResponse> MigrateVariablesToGit(ProjectResource projectResource, string branch, string commitMessage);
    }

    class ProjectBetaRepository : IProjectBetaRepository
    {
        readonly IOctopusAsyncClient client;

        public ProjectBetaRepository(IOctopusAsyncRepository repository)
        {
            client = repository.Client;
        }

        public async Task<ConvertProjectVariablesToGitResponse> MigrateVariablesToGit(ProjectResource projectResource, string branch, string commitMessage)
        {
            if (ProjectHasVariablesInGit(projectResource))
            {
                throw new NotSupportedException("Project variables have already been migrated to Git");
            }

            var command = new ConvertProjectVariablesToGitCommand
            {
                Branch = branch,
                CommitMessage = commitMessage
            };

            return await client.Post<ConvertProjectVariablesToGitCommand, ConvertProjectVariablesToGitResponse>(projectResource.Link("MigrateVariablesToGit"), command);
        }

        bool ProjectHasVariablesInGit(ProjectResource projectResource)
        {
            return projectResource.PersistenceSettings is GitPersistenceSettingsResource gitSettings && gitSettings.ConversionState.VariablesAreInGit;
        }
    }
}