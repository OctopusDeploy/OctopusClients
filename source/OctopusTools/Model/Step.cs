using System;

namespace OctopusTools.Model
{
    public class Step : Resource
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string NuGetPackageId { get; set; }
    }
}