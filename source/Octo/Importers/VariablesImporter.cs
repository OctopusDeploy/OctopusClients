using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Extensions;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Cli.Infrastructure;

namespace Octopus.Cli.Importers
{
    [Importer("variables")]
    public class VariablesImporter : BaseImporter
    {
        ValidatedImportSettings validatedImportSettings;
        public bool ReadyToImport => validatedImportSettings != null && !validatedImportSettings.ErrorList.Any();

        public VariablesImporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
            : base(repository, fileSystem, log)
        {
        }

        class ValidatedImportSettings : BaseValidatedImportSettings
        {
            public ProjectResource Project { get; set; }
            public VariableSetResource Variables { get; internal set; }
        }

        protected override async Task<bool> Validate(Dictionary<string, string> parameters)
        {
            var projectName = parameters["Project"];
            if (string.IsNullOrWhiteSpace(projectName))
            {
                Log.Error("Please specify the name of the project to export using the parameter: --project=XYZ");
                return false;
            }

            Log.Debug("Finding project: {Project:l}", projectName);
            var project = await Repository.Projects.FindByName(projectName).ConfigureAwait(false);
            if (project == null)
            {
                Log.Error("a project named", projectName);
                return false;
            }

            var variables = FileSystemImporter.Import<VariableSetResource>(FilePath, "VariableSet");
            if (variables == null)
            {
                Log.Error("Unable to deserialize the specified export file");
                return false;
            }

            validatedImportSettings = new ValidatedImportSettings
            {
                Project = project,
                Variables = variables
            };

            return true;
        }

        protected override async Task Import(Dictionary<string, string> paramDictionary)
        {
            var project = validatedImportSettings.Project;
            var variables = validatedImportSettings.Variables;

            Log.Debug("Retrieving the current variables for {Project:l}", project.Name);
            var current = await Repository.VariableSets.Get(project.VariableSetId);

            variables.Id = current.Id;
            variables.OwnerId = current.OwnerId;
            variables.Version = current.Version;

            Log.Debug("Updating the variables for {Project:l}", project.Name);
            await Repository.VariableSets.Modify(variables);

            Log.Debug("Successfully updated the variables for {Project:l}", project.Name);
        }
    }
}