using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class BuiltInRepositoryConfigurationResource : Resource
    {
        [Writeable]
        public int? DeleteUnreleasedPackagesAfterDays { get; set; }
    }
}