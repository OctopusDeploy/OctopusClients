using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using log4net;
using Octopus.Cli.Commands;
using Octopus.Cli.Extensions;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Importers
{
    [Importer("project", "ProjectWithDependencies", Description = "Imports a project from an export file")]
    public class ProjectImporter : BaseImporter
    {
        readonly protected ActionTemplateRepository actionTemplateRepository;
        ValidatedImportSettings validatedImportSettings;

        public bool ReadyToImport { get { return validatedImportSettings != null && !validatedImportSettings.ErrorList.Any(); } }
        public IEnumerable<string> ErrorList { get { return validatedImportSettings.ErrorList; } }


        class ValidatedImportSettings : BaseValidatedImportSettings
        {
            public ProjectResource Project { get; set; }
            public IDictionary<ScopeField, List<ReferenceDataItem>> ScopeValuesUsed { get; set; }
            public string ProjectGroupId { get; set; }
            public IDictionary<string, LibraryVariableSetResource> LibraryVariableSets { get; set; }
            public DeploymentProcessResource DeploymentProcess { get; set; }
            public IDictionary<string, EnvironmentResource> Environments { get; set; }
            public IDictionary<string, MachineResource> Machines { get; set; }
            public IDictionary<string, FeedResource> Feeds { get; set; }
            public IDictionary<string, ActionTemplateResource> Templates { get; set; }
            public VariableSetResource VariableSet { get; set; }
            public IEnumerable<ChannelResource> Channels { get; set; } 
            public IDictionary<string, LifecycleResource> ChannelLifecycles { get; set; } 

        }

        public ProjectImporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
            : base(repository, fileSystem, log)
        {
            actionTemplateRepository = new ActionTemplateRepository(repository.Client);
        }

        protected override bool Validate(Dictionary<string, string> paramDictionary)
        {
            var importedObject = FileSystemImporter.Import<ProjectExport>(FilePath, typeof(ProjectImporter).GetAttributeValue((ImporterAttribute ia) => ia.EntityType));

            var project = importedObject.Project;
            if (new SemanticVersion(Repository.Client.RootDocument.Version) >= new SemanticVersion(2, 6, 0, 0))
            {
                var existingLifecycle = CheckProjectLifecycle(importedObject.Lifecycle);
                if (existingLifecycle == null)
                {
                    throw new CommandException("Unable to find a lifecycle to assign to this project.");
                }
                Log.DebugFormat("Found lifecycle '{0}'", existingLifecycle.Name);
                project.LifecycleId = existingLifecycle.Id;
            }

            var variableSet = importedObject.VariableSet;
            var deploymentProcess = importedObject.DeploymentProcess;
            var nugetFeeds = importedObject.NuGetFeeds;
            var actionTemplates = importedObject.ActionTemplates ?? new List<ReferenceDataItem>();
            var libVariableSets = importedObject.LibraryVariableSets;
            var projectGroup = importedObject.ProjectGroup;
            var channels = importedObject.Channels;
            var channelLifecycles = importedObject.ChannelLifecycles;

            var scopeValuesUsed = GetScopeValuesUsed(variableSet.Variables, deploymentProcess.Steps, variableSet.ScopeValues);

            // Check Environments
            var environmentChecks = CheckEnvironmentsExist(scopeValuesUsed[ScopeField.Environment]);

            // Check Machines
            var machineChecks = CheckMachinesExist(scopeValuesUsed[ScopeField.Machine]);

            // Check NuGet Feeds
            var feedChecks = CheckNuGetFeedsExist(nugetFeeds);

            // Check Action Templates
            var templateChecks = CheckActionTemplates(actionTemplates);

            // Check Libary Variable Sets
            var libraryVariableSetChecks = CheckLibraryVariableSets(libVariableSets);

            // Check Project Group
            var projectGroupChecks = CheckProjectGroup(projectGroup);

            var channelLifecycleChecks = CheckChannelLifecycles(channelLifecycles);

            var errorList = new List<string>();

            errorList.AddRange(
                environmentChecks.MissingDependencyErrors
                    .Concat(machineChecks.MissingDependencyErrors)
                    .Concat(feedChecks.MissingDependencyErrors)
                    .Concat(templateChecks.MissingDependencyErrors)
                    .Concat(libraryVariableSetChecks.MissingDependencyErrors)
                    .Concat(projectGroupChecks.MissingDependencyErrors)
                    .Concat(channelLifecycleChecks.MissingDependencyErrors)
                );

            validatedImportSettings = new ValidatedImportSettings
            {
                Project = project,
                ProjectGroupId = projectGroupChecks.FoundDependencies.Values.First().Id,
                LibraryVariableSets = libraryVariableSetChecks.FoundDependencies,
                Environments = environmentChecks.FoundDependencies,
                Feeds = feedChecks.FoundDependencies,
                Templates = templateChecks.FoundDependencies,
                Machines = machineChecks.FoundDependencies,
                DeploymentProcess = deploymentProcess,
                ScopeValuesUsed = scopeValuesUsed,
                VariableSet = variableSet,
                Channels = channels,
                ChannelLifecycles = channelLifecycleChecks.FoundDependencies,
                ErrorList = errorList
            };

            if (validatedImportSettings.HasErrors)
            {
                var errorMessagesCsvString = string.Join(Environment.NewLine, validatedImportSettings.ErrorList);
                var errorMessage = string.Format($"The following issues were found with the provided import file: {Environment.NewLine}{errorMessagesCsvString}");
                throw new CommandException(errorMessage);
            }
            else
            {
                Log.Info("No validation errors found. Project is ready to import.");
            }

            return !validatedImportSettings.HasErrors;
        }

        protected override void Import(Dictionary<string, string> paramDictionary)
        {
            if (ReadyToImport)
            {
                Log.DebugFormat("Beginning import of project '{0}'", validatedImportSettings.Project.Name);

                var importedProject = ImportProject(validatedImportSettings.Project, validatedImportSettings.ProjectGroupId, validatedImportSettings.LibraryVariableSets);
                var importedChannels =
                    ImportProjectChannels(validatedImportSettings.Channels.ToList(), importedProject, validatedImportSettings.ChannelLifecycles)
                        .ToDictionary(k => k.Key, v => v.Value);

                ImportDeploymentProcess(validatedImportSettings.DeploymentProcess, importedProject, validatedImportSettings.Environments, validatedImportSettings.Feeds, validatedImportSettings.Templates, importedChannels);

                ImportVariableSets(validatedImportSettings.VariableSet, importedProject, validatedImportSettings.Environments, validatedImportSettings.Machines, importedChannels, validatedImportSettings.ScopeValuesUsed);

                Log.DebugFormat("Successfully imported project '{0}'", validatedImportSettings.Project.Name);
            }
            else
            {
                Log.ErrorFormat("Project is not ready to be imported.");
                if (validatedImportSettings.HasErrors)
                {
                    Log.Error("The following issues were found with the provided import file:");
                    foreach (var error in validatedImportSettings.ErrorList)
                    {
                        Log.ErrorFormat(" {0}", error);
                    }
                }
            }
        }

        protected LifecycleResource CheckProjectLifecycle(ReferenceDataItem lifecycle)
        {
            var existingLifecycles = Repository.Lifecycles.FindAll();
            if (existingLifecycles.Count == 0)
            {
                return null;
            }

            LifecycleResource existingLifecycle = null;
            if (lifecycle != null)
            {
                Log.DebugFormat("Checking that lifecycle {0} exists", lifecycle.Name);
                existingLifecycle = existingLifecycles.Find(lc => lc.Name == lifecycle.Name);
                if (existingLifecycle == null)
                {
                    Log.DebugFormat("Lifecycle {0} does not exist, default lifecycle will be used instead", lifecycle.Name);
                }
            }

            return existingLifecycle ?? existingLifecycles.FirstOrDefault();
        }

        protected Dictionary<ScopeField, List<ReferenceDataItem>> GetScopeValuesUsed(IList<VariableResource> variables, IList<DeploymentStepResource> steps, VariableScopeValues variableScopeValues)
        {
            var usedScopeValues = new Dictionary<ScopeField, List<ReferenceDataItem>>
            {
                {ScopeField.Environment, new List<ReferenceDataItem>()},
                {ScopeField.Machine, new List<ReferenceDataItem>()},
                {ScopeField.Channel, new List<ReferenceDataItem>()},
            };

            foreach (var variable in variables)
            {
                foreach (var variableScope in variable.Scope)
                {
                    switch (variableScope.Key)
                    {
                        case ScopeField.Environment:
                            var usedEnvironments = variableScope.Value;
                            foreach (var usedEnvironment in usedEnvironments)
                            {
                                var environment = variableScopeValues.Environments.Find(e => e.Id == usedEnvironment);
                                if (environment != null)
                                {
                                    usedScopeValues[ScopeField.Environment].Add(environment);
                                }
                            }
                            break;
                        case ScopeField.Machine:
                            var usedMachines = variableScope.Value;
                            foreach (var usedMachine in usedMachines)
                            {
                                var machine = variableScopeValues.Machines.Find(m => m.Id == usedMachine);
                                if (machine != null)
                                {
                                    usedScopeValues[ScopeField.Machine].Add(machine);
                                }
                            }
                            break;
                        case ScopeField.Channel:
                            var usedChannels = variableScope.Value;
                            foreach (var usedChannel in usedChannels)
                            {
                                var channel = variableScopeValues.Channels.Find(c => c.Id == usedChannel);
                                if (channel != null)
                                {
                                    usedScopeValues[ScopeField.Channel].Add(channel);
                                }
                            }
                            break;
                    }
                }
            }
            foreach (var step in steps)
            {
                foreach (var action in step.Actions)
                {
                    foreach (var usedEnvironment in action.Environments)
                    {
                        var environment = variableScopeValues.Environments.Find(e => e.Id == usedEnvironment);
                        if (environment != null && !usedScopeValues[ScopeField.Environment].Exists(env => env.Id == usedEnvironment))
                        {
                            usedScopeValues[ScopeField.Environment].Add(environment);
                        }
                    }

                    foreach (var usedChannel in action.Channels)
                    {
                        var channel = variableScopeValues.Channels.Find(c => c.Id == usedChannel);
                        if (channel != null && !usedScopeValues[ScopeField.Channel].Exists(ch => ch.Id == usedChannel))
                        {
                            usedScopeValues[ScopeField.Channel].Add(channel);
                        }
                    }
                }
            }

            return usedScopeValues;
        }

        void ImportVariableSets(VariableSetResource variableSet,
            ProjectResource importedProject,
            IDictionary<string, EnvironmentResource> environments,
            IDictionary<string, MachineResource> machines,
            IDictionary<string, ChannelResource> channels,
            IDictionary<ScopeField, List<ReferenceDataItem>> scopeValuesUsed)
        {
            Log.Debug("Importing the Projects Variable Set");
            var existingVariableSet = Repository.VariableSets.Get(importedProject.VariableSetId);

            var variables = UpdateVariables(variableSet, environments, machines, channels);
            existingVariableSet.Variables.Clear();
            existingVariableSet.Variables.AddRange(variables);

            var scopeValues = UpdateScopeValues(environments, machines, channels, scopeValuesUsed);
            existingVariableSet.ScopeValues.Actions.Clear();
            existingVariableSet.ScopeValues.Actions.AddRange(scopeValues.Actions);
            existingVariableSet.ScopeValues.Environments.Clear();
            existingVariableSet.ScopeValues.Environments.AddRange(scopeValues.Environments);
            existingVariableSet.ScopeValues.Machines.Clear();
            existingVariableSet.ScopeValues.Machines.AddRange(scopeValues.Machines);
            existingVariableSet.ScopeValues.Roles.Clear();
            existingVariableSet.ScopeValues.Roles.AddRange(scopeValues.Roles);
            existingVariableSet.ScopeValues.Machines.AddRange(scopeValues.Machines);

            Repository.VariableSets.Modify(existingVariableSet);
        }

        VariableScopeValues UpdateScopeValues(IDictionary<string, EnvironmentResource> environments, IDictionary<string, MachineResource> machines, IDictionary<string, ChannelResource> channels, IDictionary<ScopeField, List<ReferenceDataItem>> scopeValuesUsed)
        {
            var scopeValues = new VariableScopeValues();
            Log.Debug("Updating the Environments of the Variable Sets Scope Values");
            scopeValues.Environments = new List<ReferenceDataItem>();
            foreach (var environment in scopeValuesUsed[ScopeField.Environment])
            {
                var newEnvironment = environments[environment.Id];
                scopeValues.Environments.Add(new ReferenceDataItem(newEnvironment.Id, newEnvironment.Name));
            }
            Log.Debug("Updating the Machines of the Variable Sets Scope Values");
            scopeValues.Machines = new List<ReferenceDataItem>();
            foreach (var machine in scopeValuesUsed[ScopeField.Machine])
            {
                var newMachine = machines[machine.Id];
                scopeValues.Machines.Add(new ReferenceDataItem(newMachine.Id, newMachine.Name));
            }
            Log.Debug("Updating the Channels of the Variable Sets Scope Values");
            scopeValues.Channels = new List<ReferenceDataItem>();
            foreach (var channel in scopeValuesUsed[ScopeField.Channel])
            {
                var newChannel = channels[channel.Id];
                scopeValues.Channels.Add(new ReferenceDataItem(newChannel.Id, newChannel.Name));
            }
            return scopeValues;
        }

        IList<VariableResource> UpdateVariables(VariableSetResource variableSet, IDictionary<string, EnvironmentResource> environments, IDictionary<string, MachineResource> machines, IDictionary<string, ChannelResource> channels)
        {
            var variables = variableSet.Variables;

            foreach (var variable in variables)
            {
                if (variable.IsSensitive)
                {
                    Log.WarnFormat("'{0}' is a sensitive variable and it's value will be reset to a blank string, once the import has completed you will have to update it's value from the UI", variable.Name);
                    variable.Value = String.Empty;
                }
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
                        case ScopeField.Channel:
                            Log.Debug("Updating the Channel IDs of the Variables scope");
                            var oldChannelIds = scopeValue.Value;
                            var newChannelIds = new List<string>();
                            foreach (var oldChannelId in oldChannelIds)
                            {
                                newChannelIds.Add(channels[oldChannelId].Id);
                            }
                            scopeValue.Value.Clear();
                            scopeValue.Value.AddRange(newChannelIds);
                            break;
                    }
                }
            }
            return variables;
        }

        void ImportDeploymentProcess(DeploymentProcessResource deploymentProcess,
            ProjectResource importedProject,
            IDictionary<string, EnvironmentResource> environments,
            IDictionary<string, FeedResource> nugetFeeds,
            IDictionary<string, ActionTemplateResource> actionTemplates,
            IDictionary<string, ChannelResource> channels)
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
                        action.Properties["Octopus.Action.Package.NuGetFeedId"] = nugetFeeds[nugetFeedId.Value].Id;
                    }
                    if (action.Properties.ContainsKey("Octopus.Action.Template.Id"))
                    {
                        Log.Debug("Updating ID and version of Action Template");
                        var templateId = action.Properties["Octopus.Action.Template.Id"];
                        var template = actionTemplates[templateId.Value];
                        action.Properties["Octopus.Action.Template.Id"] = template.Id;
                        action.Properties["Octopus.Action.Template.Version"] = template.Version.ToString(CultureInfo.InvariantCulture);
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

                    var oldChannelIds = action.Channels;
                    var newChannelIds = new List<string>();
                    Log.Debug("Updating IDs of Channels");
                    foreach (var oldChannelId in oldChannelIds)
                    {
                        newChannelIds.Add(channels[oldChannelId].Id);
                    }
                    action.Channels.Clear();
                    action.Channels.AddRange(newChannelIds);
                }
            }
            existingDeploymentProcess.Steps.Clear();
            existingDeploymentProcess.Steps.AddRange(steps);

            Repository.DeploymentProcesses.Modify(existingDeploymentProcess);
        }

        IEnumerable<KeyValuePair<string, ChannelResource>> ImportProjectChannels(IEnumerable<ChannelResource> channels, ProjectResource importedProject, IDictionary<string, LifecycleResource> channelLifecycles)
        {
            Log.Debug("Importing the channels for the project");
            var projectChannels = Repository.Projects.GetChannels(importedProject).Items;
            var defaultChannel = projectChannels.FirstOrDefault(c => c.IsDefault);

            foreach (var channel in channels)
            {
                var existingChannel =
                    projectChannels.FirstOrDefault(c => c.Name.Equals(channel.Name, StringComparison.OrdinalIgnoreCase)) ??
                    ((channel.IsDefault && defaultChannel != null)
                        ? defaultChannel
                        : null);
                
                if (existingChannel != null)
                {
                    Log.Debug("Channel already exists, channel will be updated with new settings");
                    existingChannel.Name = channel.Name;
                    existingChannel.Description = channel.Description;
                    existingChannel.IsDefault = channel.IsDefault;
                    if (channel.LifecycleId != null)
                    {
                        existingChannel.LifecycleId = channelLifecycles[channel.LifecycleId].Id;
                    }
                    existingChannel.Rules.Clear();
                    existingChannel.Rules.AddRange(channel.Rules);

                    yield return
                        new KeyValuePair<string, ChannelResource>(channel.Id, Repository.Channels.Modify(existingChannel));
                }
                else
                {

                    Log.Debug("Channel does not exist, a new channel will be created");
                    channel.ProjectId = importedProject.Id;
                    if (channel.LifecycleId != null)
                    {
                        channel.LifecycleId = channelLifecycles[channel.LifecycleId].Id;
                    }
                    if (projectChannels.Any(c => c.IsDefault) && channel.IsDefault)
                    {
                        channel.IsDefault = false;
                    }
                    yield return
                        new KeyValuePair<string, ChannelResource>(channel.Id, Repository.Channels.Create(channel));
                }
            }

            
        }

        ProjectResource ImportProject(ProjectResource project, string projectGroupId, IDictionary<string, LibraryVariableSetResource> libraryVariableSets)
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
                existingProject.IncludedLibraryVariableSetIds.AddRange(libraryVariableSets.Values.Select(v => v.Id));
                existingProject.Slug = project.Slug;
                existingProject.VersioningStrategy.DonorPackageStepId = project.VersioningStrategy.DonorPackageStepId;
                existingProject.VersioningStrategy.Template = project.VersioningStrategy.Template;

                return Repository.Projects.Modify(existingProject);
            }
            Log.Debug("Project does not exist, a new project will be created");
            project.ProjectGroupId = projectGroupId;
            project.IncludedLibraryVariableSetIds.Clear();
            project.IncludedLibraryVariableSetIds.AddRange(libraryVariableSets.Values.Select(v => v.Id));

            return Repository.Projects.Create(project);
        }

       
        protected CheckedReferences<ProjectGroupResource> CheckProjectGroup(ReferenceDataItem projectGroup)
        {
            Log.Debug("Checking that the Project Group exist");
            var dependencies = new CheckedReferences<ProjectGroupResource>();
            var group = Repository.ProjectGroups.FindByName(projectGroup.Name);
            dependencies.Register(projectGroup.Name, projectGroup.Id, group);
            return dependencies;
        }

        protected CheckedReferences<LibraryVariableSetResource> CheckLibraryVariableSets(List<ReferenceDataItem> libraryVariableSets)
        {
            Log.Debug("Checking that all Library Variable Sets exist");
            var dependencies = new CheckedReferences<LibraryVariableSetResource>();
            var allVariableSets = Repository.LibraryVariableSets.FindAll();
            foreach (var libraryVariableSet in libraryVariableSets)
            {
                var variableSet = allVariableSets.Find(avs => avs.Name == libraryVariableSet.Name);
                dependencies.Register(libraryVariableSet.Name, libraryVariableSet.Id, variableSet);
            }
            return dependencies;
        }

        protected CheckedReferences<FeedResource> CheckNuGetFeedsExist(List<ReferenceDataItem> nugetFeeds)
        {
            Log.Debug("Checking that all NuGet Feeds exist");
            var dependencies = new CheckedReferences<FeedResource>();
            foreach (var nugetFeed in nugetFeeds)
            {
                var feed = Repository.Feeds.FindByName(nugetFeed.Name);
                dependencies.Register(nugetFeed.Name, nugetFeed.Id, feed);
            }
            return dependencies;
        }

        protected CheckedReferences<ActionTemplateResource> CheckActionTemplates(List<ReferenceDataItem> actionTemplates)
        {
            Log.Debug("Checking that all Action Templates exist");
            var dependencies = new CheckedReferences<ActionTemplateResource>();
            foreach (var actionTemplate in actionTemplates)
            {
                var template = actionTemplateRepository.FindByName(actionTemplate.Name);
                dependencies.Register(actionTemplate.Name, actionTemplate.Id, template);
            }
            return dependencies;
        }

        protected CheckedReferences<MachineResource> CheckMachinesExist(List<ReferenceDataItem> machineList)
        {
            Log.Debug("Checking that all machines exist");
            var dependencies = new CheckedReferences<MachineResource>();
            foreach (var m in machineList)
            {
                var machine = Repository.Machines.FindByName(m.Name);
                dependencies.Register(m.Name, m.Id, machine);
            }
            return dependencies;
        }

        protected CheckedReferences<EnvironmentResource> CheckEnvironmentsExist(List<ReferenceDataItem> environmentList)
        {
            Log.Debug("Checking that all environments exist");
            var dependencies = new CheckedReferences<EnvironmentResource>();
            foreach (var env in environmentList)
            {
                var environment = Repository.Environments.FindByName(env.Name);
                dependencies.Register(env.Name, env.Id, environment);
            }
            return dependencies;
        }

        protected CheckedReferences<LifecycleResource> CheckChannelLifecycles(List<ReferenceDataItem> channelLifecycles)
        {
            Log.Debug("Chekcing that all channel lifecycles exist");
            var dependencies = new CheckedReferences<LifecycleResource>();
            
            foreach (var channelLifecycle in channelLifecycles)
            {
                var lifecycle = Repository.Lifecycles.FindOne(lc => lc.Name == channelLifecycle.Name);
                dependencies.Register(channelLifecycle.Name, channelLifecycle.Id, lifecycle);
            }
            return dependencies;
        }

    }
}