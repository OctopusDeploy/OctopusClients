using Octopus.Client.Model;
using Octopus.Client.Repositories;
using System;

namespace Octopus.Client.Editors
{
    public class SubscriptionEditor : IResourceEditor<SubscriptionResource, SubscriptionEditor>
    {
        private readonly ISubscriptionRepository repository;

        public SubscriptionEditor(
           ISubscriptionRepository repository)
        {
            this.repository = repository;
        }

        public SubscriptionResource Instance { get; private set; }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId)
        {
            var existing = repository.FindByName(name);

            if (existing == null)
            {
                Instance = repository.Create(new SubscriptionResource
                {
                    Name = name,
                    Type = SubscriptionType.Event,
                    IsDisabled = isDisabled,
                    EventNotificationSubscription = eventNotificationSubscription,
                    SpaceId = spaceId
                });
            }
            else
            {
                existing.Name = name;
                existing.IsDisabled = isDisabled;
                existing.EventNotificationSubscription = eventNotificationSubscription;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public SubscriptionEditor Customize(Action<SubscriptionResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public SubscriptionEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}
