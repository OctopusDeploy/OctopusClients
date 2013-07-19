using Octopus.Client.Model;
using OctopusTools.Infrastructure;
using log4net;
using System;

namespace OctopusTools.Commands
{
    [Command("list-projects", Description = "Lists all projects")]
    public class ListProjectsCommand : ApiCommand
    {
        public ListProjectsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
        }

        protected override void Execute()
        {
            var projects = Repository.Projects.FindAll();

            Log.Info("Projects: " + projects.Count);

            foreach (var project in projects)
            {
                Log.InfoFormat(" - {0} (ID: {1})", project.Name, project.Id);
            }
        }
    }

    [Command("create-environment", Description = "Creates a project")]
    public class CreateEnvironmentCommand : ApiCommand
    {
        public CreateEnvironmentCommand(IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
        }

        public string EnvironmentName { get; set; }
        public bool IgnoreIfExists { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("name=", "The name of the environment", v => EnvironmentName = v);
            options.Add("ignoreIfExists", "If the project already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(EnvironmentName)) throw new CommandException("Please specify an environment name using the parameter: --name=XYZ");

            var env = Repository.Environments.FindByName(EnvironmentName);
            if (env != null)
            {
                if (IgnoreIfExists)
                {
                    Log.Info("The environment " + env.Name + " (ID " + env.Id + ") already exists");
                    return;
                }

                throw new CommandException("The environment " + env.Name + " (ID " + env.Id + ") already exists");
            }

            Log.Info("Creating environment: " + EnvironmentName);
            env = Repository.Environments.Create(new EnvironmentResource { Name = EnvironmentName });

            Log.Info("Environment created. ID: " + env.Id);
        }
    }

    [Command("create-project", Description = "Creates a project")]
    public class CreateProjectCommand : ApiCommand
    {
        public CreateProjectCommand(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
        }

        public string ProjectName { get; set; }
        public string ProjectGroupName { get; set; }
        public bool IgnoreIfExists { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("name=", "The name of the project", v => ProjectName = v);
            options.Add("projectGroup=", "The name of the project group to add this project to. If the group doesn't exist, it will be created.", v => ProjectGroupName = v);
            options.Add("ignoreIfExists", "If the project already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectGroupName)) throw new CommandException("Please specify a project group name using the parameter: --projectGroup=XYZ");
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --name=XYZ");

            Log.Info("Finding project group: " + ProjectGroupName);
            var group = Repository.ProjectGroups.FindByName(ProjectGroupName);
            if (group == null)
            {
                Log.Info("Project group does not exist, it will be created");
                group = Repository.ProjectGroups.Create(new ProjectGroupResource {Name = ProjectGroupName});
            }

            var project = Repository.Projects.FindByName(ProjectName);
            if (project != null)
            {
                if (IgnoreIfExists)
                {
                    Log.Info("The project " + project.Name + " (ID " + project.Id + ") already exists");
                    return;
                }

                throw new CommandException("The project " + project.Name + " (ID " + project.Id + ") already exists in this project group.");
            }

            Log.Info("Creating project: " + ProjectName);
            project = Repository.Projects.Create(new ProjectResource {Name = ProjectName, ProjectGroupId = group.Id, IsDisabled = false});

            Log.Info("Project created. ID: " + project.Id);
        }
    }
}
