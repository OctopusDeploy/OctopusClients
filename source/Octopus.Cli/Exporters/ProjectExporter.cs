using System;
using System.Collections.Generic;
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

namespace Octopus.Cli.Exporters
{
    [Exporter("project", "ProjectWithDependencies", Description = "Exports a project as JSON to a file")]
    public class ProjectExporter : BaseExporter
    {
        readonly ActionTemplateRepository actionTemplateRepository;

        public ProjectExporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
            : base(repository, fileSystem, log)
        {
            actionTemplateRepository = new ActionTemplateRepository(repository.Client);
        }

        protected override async Task Export(Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters["Name"]))
            {
                throw new CommandException("Please specify the name of the project to export using the parameter: --name=XYZ");
            }

            var projectName = parameters["Name"];

            Log.Debug("Finding project: {Project:l}", projectName);
            var project = await Repository.Projects.FindByName(projectName).ConfigureAwait(false);
            if (project == null)
                throw new CouldNotFindException("a project named", projectName);

            Log.Debug("Finding project group for project");
            var projectGroup = await Repository.ProjectGroups.Get(project.ProjectGroupId).ConfigureAwait(false);
            if (projectGroup == null)
                throw new CouldNotFindException("project group for project", project.Name);

            Log.Debug("Finding variable set for project");
            var variables = await Repository.VariableSets.Get(project.VariableSetId).ConfigureAwait(false);
            if (variables == null)
                throw new CouldNotFindException("variable set for project", project.Name);

            var channelLifecycles = new List<ReferenceDataItem>();
            var channels = new ChannelResource[0];
            if (Repository.SupportsChannels())
            {
                Log.Debug("Finding channels for project");
                var firstChannelPage = await Repository.Projects.GetChannels(project).ConfigureAwait(false);
                channels = (await firstChannelPage.GetAllPages(Repository).ConfigureAwait(false)).ToArray();

                foreach (var channel in channels.ToArray())
                {
                    if (channel.LifecycleId != null)
                    {
                        var channelLifecycle = await Repository.Lifecycles.Get(channel.LifecycleId).ConfigureAwait(false);
                        if (channelLifecycle == null)
                            throw new CouldNotFindException("Lifecycle for channel", channel.Name);
                        if (channelLifecycles.All(cl => cl.Id != channelLifecycle.Id))
                        {
                            channelLifecycles.Add(new ReferenceDataItem(channelLifecycle.Id, channelLifecycle.Name));
                        }
                    }
                }
            }

            Log.Debug("Finding deployment process for project");
            var deploymentProcess = await Repository.DeploymentProcesses.Get(project.DeploymentProcessId).ConfigureAwait(false);
            if (deploymentProcess == null)
                throw new CouldNotFindException("deployment process for project",project.Name);

            Log.Debug("Finding NuGet feed for deployment process...");
            var nugetFeeds = new List<ReferenceDataItem>();
            foreach (var step in deploymentProcess.Steps)
            {
                foreach (var action in step.Actions)
                {
                    PropertyValueResource nugetFeedId;
                    if (action.Properties.TryGetValue("Octopus.Action.Package.NuGetFeedId", out nugetFeedId))
                    {
                        Log.Debug("Finding NuGet feed for step {StepName:l}", step.Name);
                        FeedResource feed = null;
                        if (FeedCustomExpressionHelper.IsRealFeedId(nugetFeedId.Value))
                            feed = await Repository.Feeds.Get(nugetFeedId.Value).ConfigureAwait(false);
                        else
                            feed = FeedCustomExpressionHelper.CustomExpressionFeedWithId(nugetFeedId.Value);

                        if (feed == null)
                            throw new CouldNotFindException("NuGet feed for step", step.Name);

                        if (nugetFeeds.All(f => f.Id != nugetFeedId.Value))
                        {
                            nugetFeeds.Add(new ReferenceDataItem(feed.Id, feed.Name));
                        }
                    }
                }
            }

            Log.Debug("Finding action templates for project");
            var actionTemplates = new List<ReferenceDataItem>();
            foreach (var step in deploymentProcess.Steps)
            {
                foreach (var action in step.Actions)
                {
                    PropertyValueResource templateId;
                    if (action.Properties.TryGetValue("Octopus.Action.Template.Id", out templateId))
                    {
                        Log.Debug("Finding action template for step {StepName:l}", step.Name);
                        var template = await actionTemplateRepository.Get(templateId.Value).ConfigureAwait(false);
                        if (template == null)
                            throw new CouldNotFindException("action template for step", step.Name);
                        if (actionTemplates.All(t => t.Id != templateId.Value))
                        {
                            actionTemplates.Add(new ReferenceDataItem(template.Id, template.Name));
                        }
                    }
                }
            }

            var libraryVariableSets = new List<ReferenceDataItem>();
            foreach (var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                var libraryVariableSet = await Repository.LibraryVariableSets.Get(libraryVariableSetId).ConfigureAwait(false);
                if (libraryVariableSet == null)
                {
                    throw new CouldNotFindException("library variable set with Id", libraryVariableSetId);
                }
                libraryVariableSets.Add(new ReferenceDataItem(libraryVariableSet.Id, libraryVariableSet.Name));
            }

            LifecycleResource lifecycle = null;
            if (project.LifecycleId != null)
            {
                lifecycle = await Repository.Lifecycles.Get(project.LifecycleId).ConfigureAwait(false);
                if (lifecycle == null)
                {
                    throw new CouldNotFindException($"lifecycle with Id {project.LifecycleId} for project ", project.Name);
                }
            }
            
            var export = new ProjectExport
            {
                Project = project,
                ProjectGroup = new ReferenceDataItem(projectGroup.Id, projectGroup.Name),
                VariableSet = variables,
                DeploymentProcess = deploymentProcess,
                NuGetFeeds = nugetFeeds,
                ActionTemplates = actionTemplates,
                LibraryVariableSets = libraryVariableSets,
                Lifecycle = lifecycle != null ? new ReferenceDataItem(lifecycle.Id, lifecycle.Name) : null,
                Channels = channels.ToList(),
                ChannelLifecycles = channelLifecycles,
            };

            var metadata = new ExportMetadata
            {
                ExportedAt = DateTime.Now,
                OctopusVersion = Repository.Client.RootDocument.Version,
                Type = typeof (ProjectExporter).GetAttributeValue((ExporterAttribute ea) => ea.Name),
                ContainerType = typeof (ProjectExporter).GetAttributeValue((ExporterAttribute ea) => ea.EntityType)
            };
            FileSystemExporter.Export(FilePath, metadata, export);
        }
    }
}
