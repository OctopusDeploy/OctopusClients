using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.EventRetention
{
    public class EventRetentionConfigurationResource : IResource
    {

        public EventRetentionConfigurationResource()
        {
            Id = "eventretention";
        }

        public string Id { get; set; }

        [Writeable]
        public int EventRetentionDays { get; set; } = 90;

        [Writeable]
        public string CronExpression { get; set; } = "0 */4 * * *";

        public LinkCollection Links { get; set; }
    }
}
