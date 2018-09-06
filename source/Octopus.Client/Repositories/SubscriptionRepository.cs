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
        ICanIncludeSpaces<ISubscriptionRepository>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null);
    }
    
    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {

        public SubscriptionRepository(IOctopusClient client) : base(client, "Subscriptions")
        {
        }

        public SubscriptionRepository(IOctopusClient client, SpaceContextExtension spaceQueryParameters) : base(client, "Subscriptions")
        {
            SpaceContextExtension = spaceQueryParameters;
        }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled, spaceId);
        }

        public ISubscriptionRepository Including(SpaceContext spaceContext)
        {
            return new SubscriptionRepository(Client, Client.SpaceContext.Union(spaceContext).ToSpaceQueryParameters());
        }
    }
}