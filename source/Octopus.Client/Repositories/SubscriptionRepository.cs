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
        IDelete<SubscriptionResource>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled);
    }
    
    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {

        public SubscriptionRepository(IOctopusRepository repository) : base(repository, "Subscriptions")
        {
        }

        public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
        {
            GetCurrentSpaceContext().EnsureSingleSpaceContext();
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled);
        }
    }
}