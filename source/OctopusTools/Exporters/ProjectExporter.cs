using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Util;
using Octopus.Platform.Variables;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;

namespace OctopusTools.Exporters
{
    [Exporter("project", Description = "Exports a project as JSON to a file")]
    public class ProjectExporter : BaseExporter
    {
        public ProjectExporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
            : base(repository, fileSystem, log)
        {
            
        }

        protected override void Export(Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters["Name"])) throw new CommandException("Please specify the name of the project to export using the paramater: --name=XYZ");
            var projectName = parameters["Name"];

            Log.Debug("Finding project: " + projectName);
            var project = Repository.Projects.FindByName(projectName);
            if (project == null)
                throw new CommandException("Could not find project named: " + projectName);

            Log.Debug("Finding project group for project");
            var projectGroup = Repository.ProjectGroups.Get(project.ProjectGroupId);
            if (projectGroup == null)
                throw new CommandException("Could not find project group for project " + project.Name);

            Log.Debug("Finding variable set for project");
            var variables = Repository.VariableSets.Get(project.VariableSetId);
            if (variables == null)
                throw new CommandException("Could not find variable set for project " + project.Name);

            Log.Debug("Finding deployment process for project");
            var deploymentProcess = Repository.DeploymentProcesses.Get(project.DeploymentProcessId);
            if (deploymentProcess == null)
                throw new CommandException("Could not find deployment process for project " + project.Name);

            Log.Debug("Finding NuGet feed for deployment process...");

            var nugetFeeds = new List<ReferenceDataItem>();
            foreach (var step in deploymentProcess.Steps)
            {
                foreach (var action in step.Actions)
                {
                    string nugetFeedId;
                    if (action.Properties.TryGetValue("Octopus.Action.Package.NuGetFeedId", out nugetFeedId))
                    {
                        Log.Debug("Finding NuGet feed for step " + step.Name);
                        var feed = Repository.Feeds.Get(nugetFeedId);
                        if (feed == null)
                            throw new CommandException("Could not find NuGet feed for step " + step.Name);
                        if (nugetFeeds.All(f => f.Id != nugetFeedId))
                        {
                            nugetFeeds.Add(new ReferenceDataItem(feed.Id, feed.Name));
                        }

                    }

                }

            }

            var libraryVariableSets = new List<ReferenceDataItem>();
            foreach (var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                var libraryVariableSet = Repository.LibraryVariableSets.Get(libraryVariableSetId);
                if (libraryVariableSet == null)
                {
                    throw new CommandException("Could not find Library Variable Set with Library Variable Set Id " + libraryVariableSetId);
                }

                libraryVariableSets.Add(new ReferenceDataItem(libraryVariableSet.Id, libraryVariableSet.Name));
            }

            var export = new ProjectExport
            {
                Project = project,
                ProjectGroup = new ReferenceDataItem(projectGroup.Id, projectGroup.Name),
                VariableSet = variables,
                DeploymentProcess = deploymentProcess,
                NuGetFeeds = nugetFeeds,
                LibraryVariableSets = libraryVariableSets
            };

            var metadata = new
            {
                ExportedAt = DateTime.Now,
                OctopusVersion = Repository.Client.RootDocument.Version,
                Type = "project"
            };
            FileSystemExporter.Export(FilePath, metadata, export);
        }

    }
}
