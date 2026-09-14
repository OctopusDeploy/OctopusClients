using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Editors.Async
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
            => CreateOrModify(name, eventNotificationSubscription, isDisabled, CancellationToken.None);

        public async Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new SubscriptionResource
                {
                    Name = name,
                    Type = SubscriptionType.Event,
                    IsDisabled = isDisabled,
                    EventNotificationSubscription = eventNotificationSubscription,
                }, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.IsDisabled = isDisabled;
                existing.EventNotificationSubscription = eventNotificationSubscription;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public SubscriptionEditor Customize(Action<SubscriptionResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SubscriptionEditor> Save()
            => Save(CancellationToken.None);

        public async Task<SubscriptionEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
