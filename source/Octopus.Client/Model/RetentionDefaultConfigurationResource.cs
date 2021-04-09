using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RetentionDefaultConfigurationResource : Resource
    {
        [Writeable]
        public int? RetentionDays { get; set; }
    }
}