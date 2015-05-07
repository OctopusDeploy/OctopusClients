using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Platform.Model;
using Octopus.Platform.Util;
using OctopusTools.Commands;
using OctopusTools.Extensions;

namespace OctopusTools.Importers
{
    [Importer("projectDryRun", "ProjectWithDependencies", Description = "Does a 'Dry Run' and analysis of importing a project from an export file")]
    public class ProjectDryRunImporter : ProjectImporter
    {
        private readonly List<string> IssueSummary = new List<string>(); 

        public ProjectDryRunImporter(IOctopusRepository repository, IOctopusFileSystem fileSystem, ILog log)
            : base(repository, fileSystem, log)
        {
        }

        protected override void Import(Dictionary<string, string> paramDictionary)
        {
            var filePath = paramDictionary["FilePath"];
            var importedObject = FileSystemImporter.Import<ProjectExport>(filePath, typeof(ProjectImporter).GetAttributeValue((ImporterAttribute ia) => ia.EntityType));

            if (new SemanticVersion(Repository.Client.RootDocument.Version) >= new SemanticVersion(2, 6, 0, 0))
            {
                var existingLifecycle = CheckProjectLifecycle(importedObject.Lifecycle);
                if (existingLifecycle == null)
                {
                    IssueSummary.Add("Unable to find a lifecycle to assign to this project.");
                }
                else
                {
                    Log.DebugFormat("Found lifecycle '{0}'", existingLifecycle.Name);
                }
            }

            var variableSet = importedObject.VariableSet;
            var deploymentProcess = importedObject.DeploymentProcess;
            var nugetFeeds = importedObject.NuGetFeeds;
            var actionTemplates = importedObject.ActionTemplates ?? new List<ReferenceDataItem>();
            var libVariableSets = importedObject.LibraryVariableSets;
            var projectGroup = importedObject.ProjectGroup;

            var scopeValuesUsed = GetScopeValuesUsed(variableSet.Variables, deploymentProcess.Steps, variableSet.ScopeValues);

            // Check Environments
            CheckEnvironmentsExist(scopeValuesUsed[ScopeField.Environment]);

            // Check Machines
            CheckMachinesExist(scopeValuesUsed[ScopeField.Machine]);

            // Check NuGet Feeds
            CheckNuGetFeedsExist(nugetFeeds);

            // Check Action Templates
            CheckActionTemplates(actionTemplates);

            // Check Libary Variable Sets
            CheckLibraryVariableSets(libVariableSets);

            // Check Project Group
            CheckProjectGroup(projectGroup);

            Log.Debug(Environment.NewLine);
            if (IssueSummary.Any())
            {
                Log.Warn("The following issues were found with this import file:");
                IssueSummary.ForEach(Log.Warn);
            }
            else
            {
                Log.DebugFormat("No issues found with this import file");
            }
        }


        void CheckProjectGroup(ReferenceDataItem projectGroup)
        {
            Log.Debug("Checking that the Project Group exist");
            var group = Repository.ProjectGroups.FindByName(projectGroup.Name);
            if (group == null)
            {
                IssueSummary.Add("  Project Group '" + projectGroup.Name + "' does not exist");
            }
        }

        void CheckLibraryVariableSets(List<ReferenceDataItem> libraryVariableSets)
        {
            Log.Debug("Checking that all Library Variable Sets exist");
            var allVariableSets = Repository.LibraryVariableSets.FindAll();
            foreach (var libraryVariableSet in libraryVariableSets)
            {
                var variableSet = allVariableSets.Find(avs => avs.Name == libraryVariableSet.Name);
                if (variableSet == null)
                {
                    IssueSummary.Add("Library Variable Set '" + libraryVariableSet.Name + "' does not exist");
                }
            }
        }

        void CheckNuGetFeedsExist(List<ReferenceDataItem> nugetFeeds)
        {
            Log.Debug("Checking that all NuGet Feeds exist");
            foreach (var nugetFeed in nugetFeeds)
            {
                var feed = Repository.Feeds.FindByName(nugetFeed.Name);
                if (feed == null)
                {
                    IssueSummary.Add("NuGet Feed '" + nugetFeed.Name + "' does not exist");
                }
            }
        }

        void CheckActionTemplates(List<ReferenceDataItem> actionTemplates)
        {
            Log.Debug("Checking that all Action Templates exist");
            foreach (var actionTemplate in actionTemplates)
            {
                var template = actionTemplateRepository.FindByName(actionTemplate.Name);
                if (template == null)
                {
                    IssueSummary.Add("Action Template '" + actionTemplate.Name + "' does not exist");
                }
            }
        }

        void CheckMachinesExist(List<ReferenceDataItem> machineList)
        {
            Log.Debug("Checking that all machines exist");
            foreach (var m in machineList)
            {
                var machine = Repository.Machines.FindByName(m.Name);
                if (machine == null)
                {
                    IssueSummary.Add("Machine '" + m.Name + "' does not exist");
                }
            }
        }

        void CheckEnvironmentsExist(List<ReferenceDataItem> environmentList)
        {
            Log.Debug("Checking that all environments exist");
            foreach (var env in environmentList)
            {
                var environment = Repository.Environments.FindByName(env.Name);
                if (environment == null)
                {
                    IssueSummary.Add("Environment '" + env.Name + "' does not exist");
                }
            }
        }
    }
}