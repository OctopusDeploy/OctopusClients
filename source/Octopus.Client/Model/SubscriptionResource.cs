using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class SubscriptionResource : Resource, INamedResource
    {
        public SubscriptionResource()
        {
            EventNotificationSubscription = new EventNotificationSubscription();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public SubscriptionType Type { get; set; }

        [Writeable]
        public bool IsDisabled { get; set; }

        [Writeable]
        public EventNotificationSubscription EventNotificationSubscription { get; set; }
    }
}
