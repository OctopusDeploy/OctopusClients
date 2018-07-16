using System;
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
        ICanLimitToSpaces<ISubscriptionRepository>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null);
    }
    
    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {
        public SubscriptionRepository(IOctopusClient client) : base(client, "Subscriptions", null)
        {
        }

        SubscriptionRepository(IOctopusClient client, SpaceQueryParameters spaceQueryParameters): base(client, "Subscriptions", spaceQueryParameters)
        {
        }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled, spaceId);
        }

        public ISubscriptionRepository LimitTo(bool includeGlobal, params string[] spaceIds)
        {
            var newParameters = this.CreateParameters(includeGlobal, spaceIds);
            return new SubscriptionRepository(Client, newParameters);
        }
    }
}