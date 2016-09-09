using System;

namespace Octopus.Client.Model
{
    public class BuiltInRepositoryConfigurationResource : Resource
    {
        [Writeable]
        public int? DeleteUnreleasedPackagesAfterDays { get; set; }
    }
}