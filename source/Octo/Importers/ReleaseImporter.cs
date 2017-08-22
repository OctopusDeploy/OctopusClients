using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Extensions;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Importers
{
    [Importer("release", "List", Description = "Imports a projects releases from an export file")]
    public class ReleaseImporter : BaseImporter
    {
        ValidatedImportSettings validatedImportSettings;
        public bool ReadyToImport => validatedImportSettings != null && !validatedImportSettings.ErrorList.Any();

        public ReleaseImporter(IOctopusAsyncRepository repository, IOctopusFileSystem fileSystem, ILogger log)
            : base(repository, fileSystem, log)
        {
        }

        class ValidatedImportSettings : BaseValidatedImportSettings
        {
            public ProjectResource Project { get; set; }
            public IEnumerable<ReleaseResource> Releases { get; set; }
        }

        protected override async Task<bool> Validate(Dictionary<string, string> paramDictionary)
        {
            var errorList = new List<string>();

            ProjectResource project = null;
            if (string.IsNullOrWhiteSpace(paramDictionary["Project"]))
            {
                errorList.Add("Please specify the name of the project using the parameter: --project=XYZ");
            }
            else
            {
                var projectName = paramDictionary["Project"];
                project = await Repository.Projects.FindByName(projectName).ConfigureAwait(false);
                if (project == null)
                    errorList.Add("Could not find project named '" + projectName + "'");
            }

            var releases = FileSystemImporter.Import<List<ReleaseResource>>(FilePath, typeof(ReleaseImporter).GetAttributeValue((ImporterAttribute ia) => ia.EntityType));
            if (releases == null)
                errorList.Add("Unable to deserialize the specified export file");

            validatedImportSettings = new ValidatedImportSettings
            {
                Project = project,
                Releases = releases,
                ErrorList = errorList
            };

            if (validatedImportSettings.HasErrors)
            {
                Log.Error("The following issues were found with the provided input:");
                foreach (var error in validatedImportSettings.ErrorList)
                {
                    Log.Error(" {Error:l}", error);
                }
            }
            else
            {
                Log.Information("No validation errors found. Releases are ready to import.");
            }

            return !validatedImportSettings.HasErrors;
        }

        //When importing I think we want to just create a new release, using the NuGet package versions, 
        //number and release notes from the exported release
        //we can't import snapshots, so there's no point exporting them
        //instead we'll assume that they have imported the latest project settings and thus the 
        //deployment process + variables will be up to date

        protected override async Task Import(Dictionary<string, string> paramDictionary)
        {
            if (ReadyToImport)
            {
                // Start with the full list of releases from the import file, and exclude any existing releases
                var releasesToImport = validatedImportSettings.Releases.ToList();
                var releases = await Repository.Projects.GetReleases(validatedImportSettings.Project).ConfigureAwait(false);
                await releases.Paginate(Repository, page =>
                {
                    foreach (var existingRelease in page.Items)
                    {
                        if (releasesToImport.Any(r => r.Version == existingRelease.Version))
                        {
                            Log.Debug("Release '{Version:l}' already exists for project {Project:l}", existingRelease.Version , validatedImportSettings.Project.Name);
                            releasesToImport.RemoveWhere(r => r.Version == existingRelease.Version);
                        }
                    }

                    // Stop paginating if there's nothing left to import
                    return releasesToImport.Any();
                })
                .ConfigureAwait(false);

                foreach (var release in releasesToImport)
                {
                    release.ProjectId = validatedImportSettings.Project.Id;
                    Log.Debug("Creating new release '{Version:l}' for project {Project:l}", release.Version, validatedImportSettings.Project.Name);
                    await Repository.Releases.Create(release).ConfigureAwait(false);
                }

                Log.Debug("Successfully imported releases for project {Project:l}", validatedImportSettings.Project.Name);
            }
            else
            {
                Log.Error("Releases are not ready to be imported.");
                if (validatedImportSettings.HasErrors)
                {
                    Log.Error("The following issues were found with the provided input:");
                    foreach (var error in validatedImportSettings.ErrorList)
                    {
                        Log.Error(" {Error:l}", error);
                    }
                }
            }
        }
    }
}