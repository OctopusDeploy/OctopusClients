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
        ICanExpandSpaceContext<ISubscriptionRepository>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null);
    }
    
    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {

        public SubscriptionRepository(IOctopusClient client) : base(client, "Subscriptions")
        {
        }

        SubscriptionRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters): base(client, "Subscriptions")
        {
            SpaceQueryParameters = spaceQueryParameters;
        }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled, spaceId);
        }

        public ISubscriptionRepository Including(bool includeGlobal, params string[] spaceIds)
        {
            return new SubscriptionRepository(Client, new SpaceQueryParameters(includeGlobal, SpaceQueryParameters.SpaceIds.Concat(spaceIds).ToArray()));
        }
    }
}