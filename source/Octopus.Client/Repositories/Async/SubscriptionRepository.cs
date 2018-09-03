using System.Linq;
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

        SubscriptionRepository(IOctopusAsyncClient client, SpaceQueryParameters spaceQueryParameters) : base(client, "Subscriptions")
        {
            SpaceQueryParameters = spaceQueryParameters;
        }

        public Task<SubscriptionEditor> CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled, string spaceId = null)
        {
            return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled, spaceId);
        }

        public ISubscriptionRepository Including(bool includeGlobal, params string[] spaceIds)
        {
            return new SubscriptionRepository(Client, new SpaceQueryParameters(includeGlobal, SpaceQueryParameters.SpaceIds.Concat(spaceIds).ToArray()));
        }

        public ISubscriptionRepository IncludingAllSpaces()
        {
            return new SubscriptionRepository(Client, new SpaceQueryParameters(true, new[] { "all" }));
        }
    }
}