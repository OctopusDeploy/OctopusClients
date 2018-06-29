using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ISubscriptionRepository : 
        IFindByName<SubscriptionResource>, 
        ICreate<SubscriptionResource>, 
        IModify<SubscriptionResource>, 
        IGet<SubscriptionResource>, 
        IDelete<SubscriptionResource>, 
        IMixScopeRepository<SubscriptionResource>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled);
    }
    
    class SubscriptionRepository : MixScopeResourceRepository<SubscriptionResource>, ISubscriptionRepository
    {
        public SubscriptionRepository(IOctopusClient client) : base(client, "Subscriptions")
        {
        }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled);
        }
    }
}