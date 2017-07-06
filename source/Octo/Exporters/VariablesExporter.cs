using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Commands;
using Octopus.Cli.Extensions;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Exporters
{
    [Exporter("variables")]
    public class VariablesExporter : BaseExporter
    {

        public VariablesExporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
            : base(repository, fileSystem, log)
        {
        }

        protected override async Task Export(Dictionary<string, string> parameters)
        {
           var projectName = parameters["Project"];
            if (string.IsNullOrWhiteSpace(projectName))
                throw new CommandException("Please specify the name of the project to export using the parameter: --project=XYZ");


            Log.Debug("Finding project: {Project:l}", projectName);
            var project = await Repository.Projects.FindByName(projectName).ConfigureAwait(false);
            if (project == null)
                throw new CouldNotFindException("a project named", projectName);

            Log.Debug("Finding variable set for project");
            var variables = await Repository.VariableSets.Get(project.VariableSetId).ConfigureAwait(false);
            if (variables == null)
                throw new CouldNotFindException("variable set for project", project.Name);

            var metadata = new ExportMetadata
            {
                ExportedAt = DateTime.Now,
                OctopusVersion = Repository.Client.RootDocument.Version,
                Type = "variables",
                ContainerType = "VariableSet"
            };
            FileSystemExporter.Export(FilePath, metadata, variables);
        }
    }
}
