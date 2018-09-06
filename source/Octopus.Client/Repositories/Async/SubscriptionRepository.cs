using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
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
        ICanIncludeSpaces<ISubscriptionRepository>
    {
        Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null);
    }

    class SubscriptionRepository : MixedScopeBaseRepository<SubscriptionResource>, ISubscriptionRepository
    {
        public SubscriptionRepository(IOctopusAsyncClient client) : base(client, "Subscriptions")
        {
        }

        SubscriptionRepository(IOctopusAsyncClient client, SpaceContextExtension spaceQueryParameters) : base(client, "Subscriptions")
        {
            SpaceContextExtension = spaceQueryParameters;
        }

        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null)
        {
            // TODO: Maybe we should not take spaceId as an input, let it drive by the SpaceContext
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled, spaceId);
        }

        public ISubscriptionRepository Including(SpaceContext spaceContext)
        {
            return new SubscriptionRepository(Client, Client.SpaceContext.Union(spaceContext).ToSpaceQueryParameters());
        }
    }
}