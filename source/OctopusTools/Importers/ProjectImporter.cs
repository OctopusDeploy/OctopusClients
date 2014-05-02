using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Model;
using Octopus.Platform.Util;
using OctopusTools.Commands;
using OctopusTools.Exporters;
using OctopusTools.Extensions;
using OctopusTools.Infrastructure;

namespace OctopusTools.Importers
{
    [Importer("project", "ProjectWithDependencies" , Description = "Imports a project from an export file")]
    public class ProjectImporter : BaseImporter
    {
        public ProjectImporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
            : base(repository, fileSystem, log)
        {
        }

        protected override void Import(Dictionary<string, string> paramDictionary)
        {
            var filePath = paramDictionary["FilePath"];
            var importedObject = FileSystemImporter.Import<ProjectExport>(filePath, typeof(ProjectImporter).GetAttributeValue((ImporterAttribute ia) => ia.EntityType));

            var project = importedObject.Project;
            var variableSet = importedObject.VariableSet;
            var deploymentProcess = importedObject.DeploymentProcess;
            var nugetFeeds = importedObject.NuGetFeeds;
            var libVariableSets = importedObject.LibraryVariableSets;
            var projectGroup = importedObject.ProjectGroup;

            // Check Environments
            var environments = CheckEnvironmentsExist(variableSet.ScopeValues.Environments);

            // Check Machines
            var machines = CheckMachinesExist(variableSet.ScopeValues.Machines);

            // Check Roles
            var roles = CheckRolesExist(variableSet.ScopeValues.Roles);

            // Check NuGet Feeds
            var feeds = CheckNuGetFeedsExist(nugetFeeds);

            // Check Libary Variable Sets
            List<string> libraryVariableSets = CheckLibraryVariableSets(libVariableSets);

            // Check Project Group
            string projectGroupId = CheckProjectGroup(projectGroup);

            Log.DebugFormat("Beginning import of project '{0}'", project.Name);

            var importedProject = ImportProject(project, projectGroupId, libraryVariableSets);

            ImportDeploymentProcess(deploymentProcess, importedProject, environments, feeds);

            ImportVariableSets(variableSet, importedProject, environments, machines, roles);

            Log.DebugFormat("Successfully imported project '{0}'", project.Name);
        }

        private void ImportVariableSets(VariableSetResource variableSet,
            ProjectResource importedProject,
            Dictionary<string, EnvironmentResource> environments,
            Dictionary<string, MachineResource> machines,
            Dictionary<string, ReferenceDataItem> roles)
        {
            Log.Debug("Importing the Projects Variable Set");
            var existingVariableSet = Repository.VariableSets.Get(importedProject.VariableSetId);

            var variables = UpdateVariables(variableSet, environments, machines, roles);
            existingVariableSet.Variables.Clear();
            existingVariableSet.Variables.AddRange(variables);

            var scopeValues = UpdateScopeValues(variableSet, environments, machines, roles);
            existingVariableSet.ScopeValues.Actions.Clear();
            existingVariableSet.ScopeValues.Actions.AddRange(scopeValues.Actions);
            existingVariableSet.ScopeValues.Environments.Clear();
            existingVariableSet.ScopeValues.Machines.Clear();
            existingVariableSet.ScopeValues.Roles.Clear();
            existingVariableSet.ScopeValues.Machines.AddRange(scopeValues.Machines);
            existingVariableSet.ScopeValues.Roles.AddRange(scopeValues.Roles);

            Repository.VariableSets.Modify(existingVariableSet);

        }

        private VariableScopeValues UpdateScopeValues(VariableSetResource variableSet, Dictionary<string, EnvironmentResource> environments, Dictionary<string, MachineResource> machines, Dictionary<string, ReferenceDataItem> roles)
        {
            var scopeValues = new VariableScopeValues();
            Log.Debug("Updating the Environments of the Variable Sets Scope Values");
            scopeValues.Environments = new List<ReferenceDataItem>();
            foreach (var environment in variableSet.ScopeValues.Environments)
            {
                var newEnvironment = environments[environment.Id];
                scopeValues.Environments.Add(new ReferenceDataItem(newEnvironment.Id, newEnvironment.Name));
            }
            Log.Debug("Updating the Machines of the Variable Sets Scope Values");
            scopeValues.Machines = new List<ReferenceDataItem>();
            foreach (var machine in variableSet.ScopeValues.Machines)
            {
                var newMachine = machines[machine.Id];
                scopeValues.Machines.Add(new ReferenceDataItem(newMachine.Id, newMachine.Name));
            }
            Log.Debug("Updating the Roles of the Variable Sets Scope Values");
            scopeValues.Roles = new List<ReferenceDataItem>();
            foreach (var role in variableSet.ScopeValues.Roles)
            {
                scopeValues.Roles.Add(roles[role.Id]);
            }
            scopeValues.Actions = new List<ReferenceDataItem>();
            scopeValues.Actions.AddRange(variableSet.ScopeValues.Actions);
            return scopeValues;
        }

        private IList<VariableResource> UpdateVariables(VariableSetResource variableSet, Dictionary<string, EnvironmentResource> environments, Dictionary<string, MachineResource> machines, Dictionary<string, ReferenceDataItem> roles)
        {
            var variables = variableSet.Variables;

            foreach (var variable in variables)
            {
                foreach (var scopeValue in variable.Scope)
                {
                    switch (scopeValue.Key)
                    {
                        case ScopeField.Environment:
                            Log.Debug("Updating the Environment IDs of the Variables scope");
                            var oldEnvironmentIds = scopeValue.Value;
                            var newEnvironmentIds = new List<string>();
                            foreach (var oldEnvironmentId in oldEnvironmentIds)
                            {
                                newEnvironmentIds.Add(environments[oldEnvironmentId].Id);
                            }
                            scopeValue.Value.Clear();
                            scopeValue.Value.AddRange(newEnvironmentIds);
                            break;
                        case ScopeField.Machine:
                            Log.Debug("Updating the Machine IDs of the Variables scope");
                            var oldMachineIds = scopeValue.Value;
                            var newMachineIds = new List<string>();
                            foreach (var oldMachineId in oldMachineIds)
                            {
                                newMachineIds.Add(machines[oldMachineId].Id);
                            }
                            scopeValue.Value.Clear();
                            scopeValue.Value.AddRange(newMachineIds);
                            break;
                        case ScopeField.Role:
                            Log.Debug("Updating the Role IDs of the Variables scope");
                            var oldRoleIds = scopeValue.Value;
                            var newRoleIds = new List<string>();
                            foreach (var oldRoleId in oldRoleIds)
                            {
                                newRoleIds.Add(roles[oldRoleId].Id);
                            }
                            scopeValue.Value.Clear();
                            scopeValue.Value.AddRange(newRoleIds);
                            break;
                    }
                }
            }
            return variables;
        }

        private void ImportDeploymentProcess(DeploymentProcessResource deploymentProcess,
            ProjectResource importedProject,
            Dictionary<string, EnvironmentResource> environments,
            Dictionary<string, FeedResource> nugetFeeds)
        {
            Log.Debug("Importing the Projects Deployment Process");
            var existingDeploymentProcess = Repository.DeploymentProcesses.Get(importedProject.DeploymentProcessId);
            var steps = deploymentProcess.Steps;
            foreach (var step in steps)
            {
                foreach (var action in step.Actions)
                {
                    if (action.Properties.ContainsKey("Octopus.Action.Package.NuGetFeedId"))
                    {
                        Log.Debug("Updating ID of NuGet Feed");
                        var nugetFeedId = action.Properties["Octopus.Action.Package.NuGetFeedId"];
                        action.Properties["Octopus.Action.Package.NuGetFeedId"] = nugetFeeds[nugetFeedId].Id;
                    }
                    var oldEnvironmentIds = action.Environments;
                    var newEnvironmentIds = new List<string>();
                    Log.Debug("Updating IDs of Environments");
                    foreach (var oldEnvironmentId in oldEnvironmentIds)
                    {
                        newEnvironmentIds.Add(environments[oldEnvironmentId].Id);
                    }
                    action.Environments.Clear();
                    action.Environments.AddRange(newEnvironmentIds);
                }
            }
            existingDeploymentProcess.Steps.Clear();
            existingDeploymentProcess.Steps.AddRange(steps);

            Repository.DeploymentProcesses.Modify(existingDeploymentProcess);
        }

        private ProjectResource ImportProject(ProjectResource project, string projectGroupId, List<string> libraryVariableSets)
        {
            Log.Debug("Importing Project");
            var existingProject = Repository.Projects.FindByName(project.Name);
            if (existingProject != null)
            {
                Log.Debug("Project already exist, project will be updated with new settings");
                existingProject.ProjectGroupId = projectGroupId;
                existingProject.DefaultToSkipIfAlreadyInstalled = project.DefaultToSkipIfAlreadyInstalled;
                existingProject.Description = project.Description;
                existingProject.IsDisabled = project.IsDisabled;
                existingProject.IncludedLibraryVariableSetIds.Clear();
                existingProject.IncludedLibraryVariableSetIds.AddRange(libraryVariableSets);
                existingProject.Slug = project.Slug;
                existingProject.VersioningStrategy.DonorPackageStepId = project.VersioningStrategy.DonorPackageStepId;
                existingProject.VersioningStrategy.Template = project.VersioningStrategy.Template;

                return Repository.Projects.Modify(existingProject);
            }
            else
            {
                Log.Debug("Project does not exist, a new project will be created");
                project.ProjectGroupId = projectGroupId;
                project.IncludedLibraryVariableSetIds.Clear();
                project.IncludedLibraryVariableSetIds.AddRange(libraryVariableSets);

                return Repository.Projects.Create(project);
            }
        }

        private string CheckProjectGroup(ReferenceDataItem projectGroup)
        {
            Log.Debug("Checking that the Project Group exist");
            var group = Repository.ProjectGroups.FindByName(projectGroup.Name);
            if (group == null)
            {
                throw new CommandException("Project Group " + projectGroup.Name + " does not exist");
            }
            return group.Id;
        }

        private List<string> CheckLibraryVariableSets(List<ReferenceDataItem> libraryVariableSets)
        {
            Log.Debug("Checking that all Library Variable Sets exist");
            var allVariableSets = Repository.LibraryVariableSets.FindAll();
            var usedLibraryVariableSets = new List<string>();
            foreach (var libraryVariableSet in libraryVariableSets)
            {
                var variableSet = allVariableSets.Find(avs => avs.Name == libraryVariableSet.Name);
                if (variableSet == null)
                {
                    throw new CommandException("Library Variable Set " + libraryVariableSet.Name + " does not exist");
                }
                usedLibraryVariableSets.Add(variableSet.Id);
            }
            return usedLibraryVariableSets;
        }

        private Dictionary<string, FeedResource> CheckNuGetFeedsExist(List<ReferenceDataItem> nugetFeeds)
        {
            Log.Debug("Checking that all NuGet Feeds exist");
            var feeds = new Dictionary<string, FeedResource>();
            foreach (var nugetFeed in nugetFeeds)
            {
                var feed = Repository.Feeds.FindByName(nugetFeed.Name);
                if (feed == null)
                {
                    throw new CommandException("NuGet Feed " + nugetFeed.Name + " does not exist");
                }
                feeds.Add(nugetFeed.Id, feed);
            }
            return feeds;
        }

        private Dictionary<string, ReferenceDataItem> CheckRolesExist(List<ReferenceDataItem> rolesList)
        {
            Log.Debug("Checking that all roles exist");
            var allRoleNames = Repository.MachineRoles.GetAllRoleNames();
            var usedRoles = new Dictionary<string, ReferenceDataItem>();
            foreach (var role in rolesList)
            {
                if (!allRoleNames.Exists(arn => arn == role.Name))
                {
                    throw new CommandException("Role " + role.Name + " does not exist");
                }
                else
                {
                    usedRoles.Add(role.Id, role);
                }
            }
            return usedRoles;
        }

        private Dictionary<string, MachineResource> CheckMachinesExist(List<ReferenceDataItem> machineList)
        {
            Log.Debug("Checking that all machines exist");
            var machines = new Dictionary<string, MachineResource>();
            foreach (var m in machineList)
            {
                var machine = Repository.Machines.FindByName(m.Name);
                if (machine == null)
                {
                    throw new CommandException("Machine " + m.Name + " does not exist");
                }
                machines.Add(m.Id, machine);
            }
            return machines;
        }

        private Dictionary<string, EnvironmentResource> CheckEnvironmentsExist(List<ReferenceDataItem> environmentList)
        {
            Log.Debug("Checking that all environments exist");
            var usedEnvironments = new Dictionary<string, EnvironmentResource>();
            foreach (var env in environmentList)
            {
                var environment = Repository.Environments.FindByName(env.Name);
                if (environment == null)
                {
                    throw new CommandException("Environment " + env.Name + " does not exist");
                }
                usedEnvironments.Add(env.Id, environment);
            }
            return usedEnvironments;
        }

    }
}
