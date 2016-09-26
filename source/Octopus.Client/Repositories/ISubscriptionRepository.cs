using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ISubscriptionRepository : IFindByName<SubscriptionResource>, ICreate<SubscriptionResource>, IModify<SubscriptionResource>, IGet<SubscriptionResource>, IDelete<SubscriptionResource>
    {
        SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled);
    }
}
