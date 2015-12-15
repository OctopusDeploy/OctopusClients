using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using log4net;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;
using OctopusTools.Util;

namespace OctopusTools.Commands
{
    [Command("dump-deployments", new string[] {}, Description = "Writes deployments to an XML file that can be imported in Excel")]
    public class DumpDeploymentsCommand : ApiCommand
    {
        string filePath;

        public DumpDeploymentsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Dumper");
            options.Add("filePath=", "The full path and name of the export file", delegate(string v) { filePath = v; });
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new CommandException("Please specify the full path and name of the export file using the parameter: --filePath=XYZ");
            }
            Log.Info("Listing projects");
            var source = Repository.Projects.FindAll();
            var projects = source.ToDictionary(p => p.Id, p => p.Name);
            var projectsByGroup = source.ToDictionary(p => p.Id, p => p.ProjectGroupId);
            
            Log.Info("Listing project groups");
            var projectGroups = Repository.ProjectGroups.GetAll().ToDictionary(p => p.Id, p => p.Name);
            
            Log.Info("Listing environments");
            var environments = Repository.Environments.GetAll().ToDictionary(p => p.Id, p => p.Name);

            Log.Info("Dumping deployments...");
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var xml = new XmlTextWriter(new StreamWriter(fileStream)) {Formatting = Formatting.Indented};
                xml.WriteStartElement("Deployments");
                var seenBefore = new HashSet<string>();
                Repository.Deployments.Paginate(delegate(ResourceCollection<DeploymentResource> page)
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
                    Log.InfoFormat("Wrote {0:n0} of {1:n0} deployments...", seenBefore.Count, page.TotalResults);
                    return true;
                });
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