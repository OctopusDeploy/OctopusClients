using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Commands;
using Octopus.Cli.Extensions;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Importers
{
    [Importer("project", "ProjectWithDependencies", Description = "Imports a project from an export file")]
    public class ProjectImporter : BaseImporter
    {
        readonly protected ActionTemplateRepository actionTemplateRepository;
        ValidatedImportSettings validatedImportSettings;

        public bool ReadyToImport => validatedImportSettings != null && !validatedImportSettings.ErrorList.Any();
        public IEnumerable<string> ErrorList => validatedImportSettings.ErrorList;
        public bool KeepExistingProjectChannels { get; set; }

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

        public ProjectImporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
            : base(repository, fileSystem, log)
        {
            actionTemplateRepository = new ActionTemplateRepository(repository.Client);
        }

        protected override async Task<bool> Validate(Dictionary<string, string> paramDictionary)
        {
            var importedObject = FileSystemImporter.Import<ProjectExport>(FilePath, typeof(ProjectImporter).GetAttributeValue((ImporterAttribute ia) => ia.EntityType));

            var project = importedObject.Project;
            if (new SemanticVersion(Repository.Client.RootDocument.Version) >= new SemanticVersion(2, 6, 0, 0))
            {
                var existingLifecycle = await CheckProjectLifecycle(importedObject.Lifecycle).ConfigureAwait(false);
                if (existingLifecycle == null)
                {
                    throw new CommandException("Unable to find a lifecycle to assign to this project.");
                }

                Log.Debug("Found lifecycle '{Lifecycle:l}'", existingLifecycle.Name);
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

            var environmentChecksTask = CheckEnvironmentsExist(scopeValuesUsed[ScopeField.Environment]).ConfigureAwait(false);
            var machineChecksTask = CheckMachinesExist(scopeValuesUsed[ScopeField.Machine]).ConfigureAwait(false);
            var feedChecksTask = CheckNuGetFeedsExist(nugetFeeds).ConfigureAwait(false);
            var templateChecksTask = CheckActionTemplates(actionTemplates).ConfigureAwait(false);
            var libraryVariableSetChecksTask = CheckLibraryVariableSets(libVariableSets).ConfigureAwait(false);
            var projectGroupChecksTask = CheckProjectGroup(projectGroup).ConfigureAwait(false);
            var channelLifecycleChecksTask = CheckChannelLifecycles(channelLifecycles).ConfigureAwait(false);

            var environmentChecks = await environmentChecksTask;
            var machineChecks = await machineChecksTask;
            var feedChecks = await feedChecksTask;
            var templateChecks = await templateChecksTask;
            var libraryVariableSetChecks = await libraryVariableSetChecksTask;
            var projectGroupChecks = await projectGroupChecksTask;
            var channelLifecycleChecks = await channelLifecycleChecksTask;

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
                ProjectGroupId = projectGroupChecks.FoundDependencies.Values.FirstOrDefault()?.Id,
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
                Log.Information("No validation errors found. Project is ready to import.");
            }

            return !validatedImportSettings.HasErrors;
        }

        protected override async Task Import(Dictionary<string, string> paramDictionary)
        {
            if (ReadyToImport)
            {
                Log.Debug("Beginning import of project '{Project:l}'", validatedImportSettings.Project.Name);

                var importedProject = await ImportProject(validatedImportSettings.Project, validatedImportSettings.ProjectGroupId, validatedImportSettings.LibraryVariableSets).ConfigureAwait(false);

                var oldActionChannels = validatedImportSettings.DeploymentProcess.Steps.SelectMany(s => s.Actions).ToDictionary(x => x.Id, x => x.Channels.Clone());

                var importeDeploymentProcess = await ImportDeploymentProcess(validatedImportSettings.DeploymentProcess, importedProject, validatedImportSettings.Environments, validatedImportSettings.Feeds, validatedImportSettings.Templates).ConfigureAwait(false);

                var importedChannels =
                    (await ImportProjectChannels(validatedImportSettings.Channels.ToList(), importedProject, validatedImportSettings.ChannelLifecycles).ConfigureAwait(false))
                        .ToDictionary(k => k.Key, v => v.Value);

                await MapReleaseCreationStrategyChannel(importedProject, importedChannels);

                await MapChannelsToAction(importeDeploymentProcess, importedChannels, oldActionChannels);

                await ImportVariableSets(validatedImportSettings.VariableSet, importedProject, validatedImportSettings.Environments, validatedImportSettings.Machines, importedChannels, validatedImportSettings.ScopeValuesUsed).ConfigureAwait(false);

                Log.Debug("Successfully imported project '{Project:l}'", validatedImportSettings.Project.Name);
            }
            else
            {
                Log.Error("Project is not ready to be imported.");
                if (validatedImportSettings.HasErrors)
                {
                    Log.Error("The following issues were found with the provided import file:");
                    foreach (var error in validatedImportSettings.ErrorList)
                    {
                        Log.Error(" {Error:l}", error);
                    }
                }
            }
        }

        async Task MapChannelsToAction(DeploymentProcessResource importedDeploymentProcess, IDictionary<string, ChannelResource> importedChannels, IDictionary<string, ReferenceCollection> oldActionChannels)
        {
            foreach (var step in importedDeploymentProcess.Steps)
            {
                foreach (var action in step.Actions)
                {
                    Log.Debug("Setting action channels");
                    action.Channels.AddRange(oldActionChannels[action.Id].Select(oldChannelId =>  importedChannels[oldChannelId].Id));
                }
            }
            await Repository.DeploymentProcesses.Modify(importedDeploymentProcess).ConfigureAwait(false);
        }

        Task MapReleaseCreationStrategyChannel(ProjectResource importedProject, Dictionary<string, ChannelResource> channelMap)
        {
            if (importedProject.ReleaseCreationStrategy?.ChannelId == null)
                return Task.WhenAll();

            if (channelMap.ContainsKey(importedProject.ReleaseCreationStrategy.ChannelId))
            {
                importedProject.ReleaseCreationStrategy.ChannelId = channelMap[importedProject.ReleaseCreationStrategy.ChannelId].Id;
            }
            else
            {
                Log.Warning(
                    $"ReleaseCreationStrategy used channel ID '{importedProject.ReleaseCreationStrategy.ChannelId}' which was not found in imported channels. Using project default channel instead.");
                importedProject.ReleaseCreationStrategy.ChannelId = channelMap.Values.First(c => c.IsDefault).Id;
            }

            return Repository.Projects.Modify(importedProject);
        }

        protected async Task<LifecycleResource> CheckProjectLifecycle(ReferenceDataItem lifecycle)
        {
            var existingLifecycles = await Repository.Lifecycles.FindAll().ConfigureAwait(false);
            if (existingLifecycles.Count == 0)
            {
                return null;
            }

            LifecycleResource existingLifecycle = null;
            if (lifecycle != null)
            {
                Log.Debug("Checking that lifecycle {Lifecycle:l} exists", lifecycle.Name);
                existingLifecycle = existingLifecycles.Find(lc => lc.Name == lifecycle.Name);
                if (existingLifecycle == null)
                {
                    Log.Debug("Lifecycle {Lifecycle:l} does not exist, default lifecycle will be used instead", lifecycle.Name);
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

        async Task ImportVariableSets(VariableSetResource variableSet,
            ProjectResource importedProject,
            IDictionary<string, EnvironmentResource> environments,
            IDictionary<string, MachineResource> machines,
            IDictionary<string, ChannelResource> channels,
            IDictionary<ScopeField, List<ReferenceDataItem>> scopeValuesUsed)
        {
            Log.Debug("Importing the Projects Variable Set");
            var existingVariableSet = await Repository.VariableSets.Get(importedProject.VariableSetId).ConfigureAwait(false);

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

            await Repository.VariableSets.Modify(existingVariableSet).ConfigureAwait(false);
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
                    Log.Warning("{Variable} is a sensitive variable and it's value will be reset to a blank string, once the import has completed you will have to update it's value from the UI", variable.Name);
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

        async Task<DeploymentProcessResource> ImportDeploymentProcess(DeploymentProcessResource deploymentProcess,
            ProjectResource importedProject,
            IDictionary<string, EnvironmentResource> environments,
            IDictionary<string, FeedResource> nugetFeeds,
            IDictionary<string, ActionTemplateResource> actionTemplates)
        {
            Log.Debug("Importing the Projects Deployment Process");
            var existingDeploymentProcess = await Repository.DeploymentProcesses.Get(importedProject.DeploymentProcessId).ConfigureAwait(false);
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

                    // Make sure source channels are clear, will be added later
                    action.Channels.Clear();
                }
            }
            existingDeploymentProcess.Steps.Clear();
            existingDeploymentProcess.Steps.AddRange(steps);

            return await Repository.DeploymentProcesses.Modify(existingDeploymentProcess).ConfigureAwait(false);
        }

        async Task<IReadOnlyList<KeyValuePair<string, ChannelResource>>> ImportProjectChannels(List<ChannelResource> channels, ProjectResource importedProject, IDictionary<string, LifecycleResource> channelLifecycles)
        {
            Log.Debug("Importing the channels for the project");
            var allChannels = await Repository.Projects.GetChannels(importedProject).ConfigureAwait(false);
            var projectChannels = (await allChannels.GetAllPages(Repository).ConfigureAwait(false)).ToArray();
            var defaultChannel = projectChannels.ToArray().FirstOrDefault(c => c.IsDefault);
            var newDefaultChannel = channels.FirstOrDefault(nc => nc.IsDefault);
            var defaultChannelUpdated = false;

            var results = new List<KeyValuePair<string, ChannelResource>>();

            foreach (var channel in channels)
            {
                var existingChannel =
                    projectChannels.FirstOrDefault(c => c.Name.Equals(channel.Name, StringComparison.OrdinalIgnoreCase));

                if (existingChannel != null)
                {
                    Log.Debug($"Channel '{existingChannel.Name}' already exists, channel will be updated with new settings");
                    existingChannel.Name = channel.Name;
                    existingChannel.Description = channel.Description;
                    existingChannel.IsDefault = channel.IsDefault;
                    if (channel.LifecycleId != null)
                    {
                        existingChannel.LifecycleId = channelLifecycles[channel.LifecycleId].Id;
                    }
                    if (existingChannel.IsDefault && !existingChannel.Name.Equals(newDefaultChannel?.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        existingChannel.IsDefault = false;
                    }
                    existingChannel.Rules.Clear();
                    existingChannel.Rules.AddRange(channel.Rules);
                    if (existingChannel.Name.Equals(defaultChannel?.Name))
                    {
                        defaultChannelUpdated = true;
                    }
                    var modified = await Repository.Channels.Modify(existingChannel).ConfigureAwait(false);
                    results.Add(new KeyValuePair<string, ChannelResource>(channel.Id, modified));
                }
                else
                {
                    Log.Debug($"Channel `{channel.Name}` does not exist, a new channel will be created");
                    channel.ProjectId = importedProject.Id;
                    if (channel.LifecycleId != null)
                    {
                        channel.LifecycleId = channelLifecycles[channel.LifecycleId].Id;
                    }

                    var created = await Repository.Channels.Create(channel).ConfigureAwait(false);
                    results.Add(new KeyValuePair<string, ChannelResource>(channel.Id, created));
                }
            }

            if (!KeepExistingProjectChannels && !defaultChannelUpdated)
            {
                await Repository.Channels.Delete(defaultChannel).ConfigureAwait(false);
            }

            return results;
        }

        async Task<ProjectResource> ImportProject(ProjectResource project, string projectGroupId, IDictionary<string, LibraryVariableSetResource> libraryVariableSets)
        {
            Log.Debug("Importing Project");
            var existingProject = await Repository.Projects.FindByName(project.Name).ConfigureAwait(false);
            if (existingProject != null)
            {
                KeepExistingProjectChannels = true;
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

                return await Repository.Projects.Modify(existingProject).ConfigureAwait(false);
            }
            Log.Debug("Project does not exist, a new project will be created");
            project.ProjectGroupId = projectGroupId;
            project.IncludedLibraryVariableSetIds.Clear();
            project.IncludedLibraryVariableSetIds.AddRange(libraryVariableSets.Values.Select(v => v.Id));

            return await Repository.Projects.Create(project).ConfigureAwait(false);
        }

        protected async Task<CheckedReferences<ProjectGroupResource>> CheckProjectGroup(ReferenceDataItem projectGroup)
        {
            Log.Debug("Checking that the Project Group exist");
            var dependencies = new CheckedReferences<ProjectGroupResource>();
            var group = await Repository.ProjectGroups.FindByName(projectGroup.Name).ConfigureAwait(false);
            dependencies.Register(projectGroup.Name, projectGroup.Id, group);
            return dependencies;
        }

        protected async Task<CheckedReferences<LibraryVariableSetResource>> CheckLibraryVariableSets(List<ReferenceDataItem> libraryVariableSets)
        {
            Log.Debug("Checking that all Library Variable Sets exist");
            var dependencies = new CheckedReferences<LibraryVariableSetResource>();
            var allVariableSets = await Repository.LibraryVariableSets.FindAll().ConfigureAwait(false);
            foreach (var libraryVariableSet in libraryVariableSets)
            {
                var variableSet = allVariableSets.Find(avs => avs.Name == libraryVariableSet.Name);
                dependencies.Register(libraryVariableSet.Name, libraryVariableSet.Id, variableSet);
            }
            return dependencies;
        }

        protected async Task<CheckedReferences<FeedResource>> CheckNuGetFeedsExist(List<ReferenceDataItem> nugetFeeds)
        {
            Log.Debug("Checking that all NuGet Feeds exist");
            var dependencies = new CheckedReferences<FeedResource>();
            foreach (var nugetFeed in nugetFeeds)
            {
                FeedResource feed = null;
                if (FeedCustomExpressionHelper.IsRealFeedId(nugetFeed.Id))
                    feed = await Repository.Feeds.FindByName(nugetFeed.Name).ConfigureAwait(false);
                else
                    feed = FeedCustomExpressionHelper.CustomExpressionFeedWithId(nugetFeed.Id);

                dependencies.Register(nugetFeed.Name, nugetFeed.Id, feed);
            }
            return dependencies;
        }

        protected async Task<CheckedReferences<ActionTemplateResource>> CheckActionTemplates(List<ReferenceDataItem> actionTemplates)
        {
            Log.Debug("Checking that all Action Templates exist");
            var dependencies = new CheckedReferences<ActionTemplateResource>();
            foreach (var actionTemplate in actionTemplates)
            {
                var template = await actionTemplateRepository.FindByName(actionTemplate.Name).ConfigureAwait(false);
                dependencies.Register(actionTemplate.Name, actionTemplate.Id, template);
            }
            return dependencies;
        }

        protected async Task<CheckedReferences<MachineResource>> CheckMachinesExist(List<ReferenceDataItem> machineList)
        {
            Log.Debug("Checking that all machines exist");
            var dependencies = new CheckedReferences<MachineResource>();
            foreach (var m in machineList)
            {
                var machine = await Repository.Machines.FindByName(m.Name).ConfigureAwait(false);
                dependencies.Register(m.Name, m.Id, machine);
            }
            return dependencies;
        }

        protected async Task<CheckedReferences<EnvironmentResource>> CheckEnvironmentsExist(List<ReferenceDataItem> environmentList)
        {
            Log.Debug("Checking that all environments exist");
            var dependencies = new CheckedReferences<EnvironmentResource>();
            foreach (var env in environmentList)
            {
                var environment = await Repository.Environments.FindByName(env.Name).ConfigureAwait(false);
                dependencies.Register(env.Name, env.Id, environment);
            }
            return dependencies;
        }

        protected async Task<CheckedReferences<LifecycleResource>> CheckChannelLifecycles(List<ReferenceDataItem> channelLifecycles)
        {
            Log.Debug("Checking that all channel lifecycles exist");
            var dependencies = new CheckedReferences<LifecycleResource>();

            foreach (var channelLifecycle in channelLifecycles)
            {
                var lifecycle = await Repository.Lifecycles.FindOne(lc => lc.Name == channelLifecycle.Name).ConfigureAwait(false);
                dependencies.Register(channelLifecycle.Name, channelLifecycle.Id, lifecycle);
            }
            return dependencies;
        }

    }
}