using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface ISubscriptionRepository : 
        IFindByName<SubscriptionResource>, 
        ICreate<SubscriptionResource>, 
        IModify<SubscriptionResource>, 
        IGet<SubscriptionResource>, 
        IDelete<SubscriptionResource>,
        ICanExtendSpaceContext<ISubscriptionRepository>
    {
        Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled);
    }

    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {
        public SubscriptionRepository(IOctopusAsyncClient client) : base(client, "Subscriptions")
        {
        }

        SubscriptionRepository(IOctopusAsyncClient client, SpaceContext spaceContext) : base(client, "Subscriptions", spaceContext)
        {
        }

        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
        {
            GetCurrentSpaceContext().EnsureSingleSpaceContext();
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled);
        }

        public ISubscriptionRepository Including(SpaceContext spaceContext)
        {
            return new SubscriptionRepository(Client, ExtendSpaceContext(spaceContext));
        }
    }
}