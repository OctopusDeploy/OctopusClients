using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Deployment
{
    [Command("dump-deployments", new string[] {}, Description = "Writes deployments to an XML file that can be imported in Excel")]
    public class DumpDeploymentsCommand : ApiCommand
    {
        string filePath;

        public DumpDeploymentsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Dumper");
            options.Add("filePath=", "The full path and name of the export file", delegate(string v) { filePath = v; });
        }

        protected override async Task Execute()
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new CommandException("Please specify the full path and name of the export file using the parameter: --filePath=XYZ");
            }
            commandOutputProvider.Information("Listing projects, project groups and environments");
            var projectsTask = Repository.Projects.FindAll().ConfigureAwait(false);
            var projectGroupsTask =  Repository.ProjectGroups.GetAll().ConfigureAwait(false);
            var environmentsTask = Repository.Environments.GetAll().ConfigureAwait(false);

            var projects = (await projectsTask).ToDictionary(p => p.Id, p => p.Name);
            var projectsByGroup = (await projectsTask).ToDictionary(p => p.Id, p => p.ProjectGroupId);
            var projectGroups = (await projectGroupsTask).ToDictionary(p => p.Id, p => p.Name);
            var environments = (await environmentsTask).ToDictionary(p => p.Id, p => p.Name);

            commandOutputProvider.Information("Dumping deployments...");
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var xmlWriterSettings = new XmlWriterSettings() {Indent = true};
                var xml = XmlWriter.Create(new StreamWriter(fileStream), xmlWriterSettings);
                xml.WriteStartElement("Deployments");
                var seenBefore = new HashSet<string>();
                await Repository.Deployments.Paginate(delegate(ResourceCollection<DeploymentResource> page)
                {
                    foreach (var current in page.Items)
                    {
                        if (seenBefore.Contains(current.Id)) continue;
                        seenBefore.Add(current.Id);
                        xml.WriteStartElement("Deployment");
                        xml.WriteElementString("Environment", GetName(current.EnvironmentId, environments));
                        xml.WriteElementString("Project", GetName(current.ProjectId, projects));
                        xml.WriteElementString("ProjectGroup", GetName(GetProjectGroupId(current.ProjectId, projectsByGroup), projectGroups));
                        xml.WriteElementString("Created", current.Created.ToString("s"));
                        xml.WriteElementString("Name", current.Name);
                        xml.WriteElementString("Id", current.Id);
                        xml.WriteEndElement();
                    }
                    commandOutputProvider.Information("Wrote {Count:n0} of {Total:n0} deployments...", seenBefore.Count, page.TotalResults);
                    return true;
                })
                .ConfigureAwait(false);
                xml.WriteEndElement();
                xml.Flush();
            }
        }

        static string GetProjectGroupId(string projectId, IDictionary<string, string> projectsByGroup)
        {
            string result;
            if (string.IsNullOrWhiteSpace(projectId))
            {
                result = null;
            }
            else
            {
                string text;
                result = projectsByGroup.TryGetValue(projectId, out text) ? text : null;
            }
            return result;
        }

        static string GetName(string id, IDictionary<string, string> dictionary)
        {
            string result;
            if (string.IsNullOrWhiteSpace(id))
            {
                result = null;
            }
            else
            {
                string text;
                result = dictionary.TryGetValue(id, out text) ? text : null;
            }
            return result;
        }
    }
}