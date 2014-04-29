using log4net;
using Newtonsoft.Json;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Octopus.Platform.Util;
using System.IO;

namespace OctopusTools.Commands
{
    [Command("export-project", Description = "Exports a projects settings, variables and deployment process.")]
    public class ExportProjectCommand : ApiCommand
    {
        readonly IOctopusFileSystem fileSystem;
        public ExportProjectCommand(IOctopusFileSystem fileSystem, IOctopusRepositoryFactory repositoryFactory, ILog log)
            : base(repositoryFactory, log)
        {
            this.fileSystem = fileSystem;
        }

        public string ProjectName { get; set; }

        public string FilePath { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("filePath=", "Full path and name of export file", v => FilePath = v);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify the project name using the parameter: --project=XYZ");
            if (string.IsNullOrWhiteSpace(FilePath)) throw new CommandException("Please specify the full path and name for the export file using the parameter: --filePath=XYZ");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CommandException("Could not find project named: " + ProjectName);
            
            Log.Debug("Finding project group for project");
            var projectGroup = Repository.ProjectGroups.Get(project.ProjectGroupId);
            if(projectGroup == null)
                throw new CommandException("Could not find project group for project " + project.Name);

            Log.Debug("Finding variable set for project");
            var variables = Repository.VariableSets.Get(project.VariableSetId);
            if (variables == null)
                throw new CommandException("Could not find variable set for project " + project.Name);

            Log.Debug("Finding deployment process for project");
            var deploymentProcess = Repository.DeploymentProcesses.Get(project.DeploymentProcessId);
            if (deploymentProcess == null)
                throw new CommandException("Could not find deployment process for project " + project.Name);

            Log.Debug("Finding NuGet feed for deployment process");

            var nugetFeeds = new List<ReferenceDataItem>();
            foreach(var step in deploymentProcess.Steps)
            {
                foreach(var action in step.Actions)
                {
                    string nugetFeedId;
                    if (action.Properties.TryGetValue("Octopus.Action.Package.NuGetFeedId", out nugetFeedId))
                    {
                        Log.Debug("Finding NuGet feed for step " + step.Name);
                        var feed = Repository.Feeds.Get(nugetFeedId);
                        if (feed == null)
                            throw new CommandException("Could not find NuGet feed for step " + step.Name);
                        if (!nugetFeeds.Any(f => f.Id == nugetFeedId))
                        {
                            nugetFeeds.Add(new ReferenceDataItem(feed.Id, feed.Name));
                        }
                    }
                }
            }

            var libraryVariableSets = new List<ReferenceDataItem>();
            foreach(var libraryVariableSetId in project.IncludedLibraryVariableSetIds)
            {
                var libraryVariableSet = Repository.LibraryVariableSets.Get(libraryVariableSetId);
                if(libraryVariableSet == null)
                {
                    throw new CommandException("Could not find Library Variable Set with Library Variable Set Id " + libraryVariableSetId);
                }
                libraryVariableSets.Add(new ReferenceDataItem(libraryVariableSet.Id, libraryVariableSet.Name));
            }

            var export = JsonConvert.SerializeObject(new ProjectExportObject
            {
                Project = project,
                ProjectGroup = new ReferenceDataItem(projectGroup.Id, projectGroup.Name),
                VariableSet = variables,
                DeploymentProcess = deploymentProcess,
                NuGetFeeds = nugetFeeds,
                LibraryVariableSets = libraryVariableSets
            });

            try
            {
                using (var fileStream = fileSystem.OpenFile(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    var bytes = Encoding.UTF8.GetBytes(export);
                    fileStream.Write(bytes, 0, bytes.Length);
                }
                Log.DebugFormat("Export file {0} successfully created.", FilePath);
            }
            catch(Exception ex)
            {
                Log.DebugFormat("Failed to write file {0} to file system. {1}", FilePath, ex.Message);
            }
        }
    }
}
