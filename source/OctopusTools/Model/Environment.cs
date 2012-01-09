using System;
using System.Collections.Generic;
using System.Linq;

namespace OctopusTools.Model
{
    public class Resource
    {
        public Dictionary<string, string> Links { get; set; }

        public string Link(string name)
        {
            var links = Links.Select(kvp => new { kvp.Key, kvp.Value });

            var link = links.FirstOrDefault(l => string.Equals(l.Key, name, StringComparison.InvariantCultureIgnoreCase));
            if (link == null)
            {
                throw new Exception(string.Format("The document does not define a link for '{0}'", name));
            }

            return link.Value;
        }
    }

    public class DeploymentEnvironment : Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Project : Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Step : Resource
    {
        public string NuGetPackageId { get; set; }
    }

    public class Deployment : Resource
    {
        public int Id { get; set; }
        public int ReleaseId { get; set; }
        public int EnvironmentId { get; set; }
        public bool ForceRedeployment { get; set; }
        public string Name { get; set; }
    }

    public class Release : Resource
    {
        public int Id { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime Assembled { get; set; }
        public string AssembledBy { get; set; }
        public string Version { get; set; }
        public string Href { get; set; }
        public PackageVersion[] PackageVersions { get; set; }
    }

    public class PackageVersion
    {
        public string Id { get; set; }
        public string Version { get; set; }
    }
}