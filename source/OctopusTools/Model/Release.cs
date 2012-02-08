using System;

namespace OctopusTools.Model
{
    public class Release : Resource
    {
        public int Id { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime Assembled { get; set; }
        public string AssembledBy { get; set; }
        public string Version { get; set; }
        public string Href { get; set; }
        public PackageVersion[] PackageVersions { get; set; }
        public SelectedPackage[] SelectedPackages { get; set; }
    }
}