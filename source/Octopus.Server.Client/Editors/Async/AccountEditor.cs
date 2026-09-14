using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Accounts.Usages;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AccountEditor<TAccountResource, TAccountEditor> : IResourceEditor<TAccountResource, TAccountEditor>
        where TAccountResource : AccountResource, new()
        where TAccountEditor : AccountEditor<TAccountResource, TAccountEditor>
    {
        protected readonly IAccountRepository Repository;

        public AccountEditor(IAccountRepository repository)
        {
            Repository = repository;
        }

        public TAccountResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TAccountEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<TAccountEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await Repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = (TAccountResource)await Repository.Create(new TAccountResource
                {
                    Name = name
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (!(existing is TAccountResource))
                {
                    throw new ArgumentException($"An account with that name exists but is not of type {typeof(TAccountResource).Name}");
                }

                Instance = (TAccountResource)existing;
            }

            return (TAccountEditor)this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TAccountEditor> FindByName(string name)
            => FindByName(name, CancellationToken.None);

        public async Task<TAccountEditor> FindByName(string name, CancellationToken cancellationToken)
        {
            var existing = await Repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                throw new ArgumentException($"An account with the name {name} could not be found");
            }
            else
            {
                if (!(existing is TAccountResource))
                {
                    throw new ArgumentException($"An account with that name exists but is not of type {typeof(TAccountResource).Name}");
                }

                Instance = (TAccountResource)existing;
            }

            return (TAccountEditor)this;
        }

        public virtual TAccountEditor Customize(Action<TAccountResource> customize)
        {
            customize?.Invoke(Instance);
            return (TAccountEditor)this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public virtual Task<TAccountEditor> Save()
            => Save(CancellationToken.None);

        public virtual async Task<TAccountEditor> Save(CancellationToken cancellationToken)
        {
            Instance = (TAccountResource)await Repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return (TAccountEditor)this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<AccountUsageResource> Usages()
            => Usages(CancellationToken.None);

        public Task<AccountUsageResource> Usages(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<AccountUsageResource>(Instance.Link("Usages"), cancellationToken);
        }
    }
}
