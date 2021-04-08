using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RetentionDefaultConfigurationResource : Resource
    {
        public RetentionDefaultConfigurationResource()
        {
            Id = "retention-default";
        }

        [Writeable]
        public int? RetentionDays { get; set; }
    }
}