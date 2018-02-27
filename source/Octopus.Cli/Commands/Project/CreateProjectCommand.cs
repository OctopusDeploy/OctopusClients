using System;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Project
{
    [Command("create-project", Description = "Creates a project")]
    public class CreateProjectCommand : ApiCommand, ISupportFormattedOutput
    {
        ProjectResource project;
        ProjectGroupResource projectGroup;
        private bool projectGroupCreated = false;

        public CreateProjectCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
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
        
        public async Task Request()
        {
            if (string.IsNullOrWhiteSpace(ProjectGroupName)) throw new CommandException("Please specify a project group name using the parameter: --projectGroup=XYZ");
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --name=XYZ");
            if (string.IsNullOrWhiteSpace(LifecycleName)) throw new CommandException("Please specify a lifecycle name using the parameter: --lifecycle=XYZ");

            commandOutputProvider.Information("Finding project group: {Group:l}", ProjectGroupName);

            projectGroup = await Repository.ProjectGroups.FindByName(ProjectGroupName).ConfigureAwait(false);
            if (projectGroup == null)
            {
                commandOutputProvider.Information("Project group does not exist, it will be created");
                projectGroup = await Repository.ProjectGroups.Create(new ProjectGroupResource { Name = ProjectGroupName }).ConfigureAwait(false);
                projectGroupCreated = true;
            }

            commandOutputProvider.Information("Finding lifecycle: {Lifecycle:l}", LifecycleName);
            var lifecycle = await Repository.Lifecycles.FindOne(l => l.Name.Equals(LifecycleName, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);
            if (lifecycle == null)
                throw new CommandException($"The lifecycle {LifecycleName} does not exist.");

            
            project = await Repository.Projects.FindByName(ProjectName).ConfigureAwait(false);
            if (project != null)
            {
                if (IgnoreIfExists)
                {
                    commandOutputProvider.Information("The project {Project:l} (ID {Id:l}) already exists", project.Name, project.Id);
                    return;
                }

                throw new CommandException($"The project {project.Name} (ID {project.Id}) already exists in this project group.");
            }

            commandOutputProvider.Information("Creating project: {Project:l}", ProjectName);
            project = await Repository.Projects.Create(new ProjectResource { Name = ProjectName, ProjectGroupId = projectGroup.Id, IsDisabled = false, LifecycleId = lifecycle.Id }).ConfigureAwait(false);
        }

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("Project created. ID: {Id:l}", project.Id);
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                Project = new
                {
                    project.Id,
                    project.Name
                },
                Group = new
                {
                    projectGroup.Id,
                    projectGroup.Name,
                    NewGroupCreated = projectGroupCreated
                }
            });
        }
    }
}