using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ISubscriptionRepository :
        IFindByName<SubscriptionResource>,
        ICreate<SubscriptionResource>,
        IModify<SubscriptionResource>,
        IGet<SubscriptionResource>,
        IDelete<SubscriptionResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled);
        Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, CancellationToken cancellationToken);
    }

    class SubscriptionRepository : BasicRepository<SubscriptionResource>, ISubscriptionRepository
    {
        public SubscriptionRepository(IOctopusAsyncRepository repository) : base(repository, "Subscriptions")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
            => CreateOrModify(name, eventNotificationSubscription, isDisabled, CancellationToken.None);

        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, CancellationToken cancellationToken)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled);
        }

    }
}
