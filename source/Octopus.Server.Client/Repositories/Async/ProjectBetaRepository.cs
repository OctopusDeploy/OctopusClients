using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Git;

namespace Octopus.Client.Repositories.Async
{
    public interface IProjectBetaRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ConvertProjectVariablesToGitResponse> MigrateVariablesToGit(ProjectResource projectResource, string branch, string commitMessage);
        Task<ConvertProjectVariablesToGitResponse> MigrateVariablesToGit(ProjectResource projectResource, string branch, string commitMessage, CancellationToken cancellationToken);
    }

    class ProjectBetaRepository : IProjectBetaRepository
    {
        readonly IOctopusAsyncClient client;

        public ProjectBetaRepository(IOctopusAsyncRepository repository)
        {
            client = repository.Client;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ConvertProjectVariablesToGitResponse> MigrateVariablesToGit(ProjectResource projectResource, string branch, string commitMessage)
            => MigrateVariablesToGit(projectResource, branch, commitMessage, CancellationToken.None);

        public async Task<ConvertProjectVariablesToGitResponse> MigrateVariablesToGit(ProjectResource projectResource, string branch, string commitMessage, CancellationToken cancellationToken)
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

            if (!projectResource.HasLink("MigrateVariablesToGit"))
            {
                throw new NotSupportedException("Git variables migration is not available for this project");
            }

            return await client.Post<ConvertProjectVariablesToGitCommand, ConvertProjectVariablesToGitResponse>(projectResource.Link("MigrateVariablesToGit"), command, cancellationToken);
        }

        bool ProjectHasVariablesInGit(ProjectResource projectResource)
        {
            return projectResource.PersistenceSettings is GitPersistenceSettingsResource { ConversionState: { VariablesAreInGit: true } };
        }
    }
}
