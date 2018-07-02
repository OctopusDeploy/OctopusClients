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
        IDelete<SubscriptionResource>,
        IMixScopeRepository<SubscriptionResource>
    {
        Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null);
    }

    class SubscriptionRepository : MixScopeResourceRepository<SubscriptionResource>, ISubscriptionRepository
    {
        public SubscriptionRepository(IOctopusAsyncClient client) : base(client, "Subscriptions")
        {
        }

        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled, spaceId);
        }
    }
}