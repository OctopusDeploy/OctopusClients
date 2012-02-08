using System;

namespace OctopusTools.Model
{
    public class Step : Resource
    {
        public int Id { get; set; }
        public string NuGetPackageId { get; set; }
    }
}