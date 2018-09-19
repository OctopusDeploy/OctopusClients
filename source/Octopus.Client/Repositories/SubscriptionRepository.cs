using System;
using System.Linq;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface ISubscriptionRepository : 
        IFindByName<SubscriptionResource>, 
        ICreate<SubscriptionResource>, 
        IModify<SubscriptionResource>, 
        IGet<SubscriptionResource>, 
        IDelete<SubscriptionResource>, 
        ICanExtendSpaceContext<ISubscriptionRepository>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled);
    }
    
    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {

        public SubscriptionRepository(IOctopusClient client) : base(client, "Subscriptions")
        {
        }

        SubscriptionRepository(IOctopusClient client, SpaceContext spaceContext) : base(client, "Subscriptions", spaceContext)
        {
        }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
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