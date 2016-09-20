using System;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-project", Description = "Creates a project")]
    public class CreateProjectCommand : ApiCommand
    {
        public CreateProjectCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Project creation");
            options.Add("name=", "The name of the project", v => ProjectName = v);
            options.Add("projectGroup=", "The name of the project group to add this project to. If the group doesn't exist, it will be created.", v => ProjectGroupName = v);
            options.Add("lifecycle=", "The name of the lifecycle that the project will use.", v=> LifecycleName = v);
            options.Add("ignoreIfExists", "If the project already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        public string ProjectName { get; set; }
        public string ProjectGroupName { get; set; }
        public bool IgnoreIfExists { get; set; }
        public string LifecycleName { get; set; }

        protected override async Task Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectGroupName)) throw new CommandException("Please specify a project group name using the parameter: --projectGroup=XYZ");
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --name=XYZ");
            if (string.IsNullOrWhiteSpace(LifecycleName)) throw new CommandException("Please specify a lifecycle name using the parameter: --lifecycle=XYZ");

            Log.Information("Finding project group: " + ProjectGroupName);
            var group = await Repository.ProjectGroups.FindByName(ProjectGroupName).ConfigureAwait(false);
            if (group == null)
            {
                Log.Information("Project group does not exist, it will be created");
                group = await Repository.ProjectGroups.Create(new ProjectGroupResource {Name = ProjectGroupName}).ConfigureAwait(false);
            }

            Log.Information("Finding lifecycle: " + LifecycleName);
            var lifecycle = await Repository.Lifecycles.FindOne(l => l.Name.Equals(LifecycleName, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);
            if (lifecycle == null)
                throw new CommandException("The lifecycle " + LifecycleName + " does not exist.");

            var project = await Repository.Projects.FindByName(ProjectName).ConfigureAwait(false);
            if (project != null)
            {
                if (IgnoreIfExists)
                {
                    Log.Information("The project " + project.Name + " (ID " + project.Id + ") already exists");
                    return;
                }

                throw new CommandException("The project " + project.Name + " (ID " + project.Id + ") already exists in this project group.");
            }
            
            Log.Information("Creating project: " + ProjectName);
            project = await Repository.Projects.Create(new ProjectResource {Name = ProjectName, ProjectGroupId = @group.Id, IsDisabled = false, LifecycleId = lifecycle.Id}).ConfigureAwait(false);

            Log.Information("Project created. ID: " + project.Id);
        }
    }
}