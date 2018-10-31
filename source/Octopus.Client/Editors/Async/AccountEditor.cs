using System;
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

        public async Task<TAccountEditor> CreateOrModify(string name)
        {
            var existing = await Repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = (TAccountResource)await Repository.Create(new TAccountResource
                {
                    Name = name
                }).ConfigureAwait(false);
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

        public async Task<TAccountEditor> FindByName(string name)
        {
            var existing = await Repository.FindByName(name).ConfigureAwait(false);
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

        public virtual async Task<TAccountEditor> Save()
        {
            Instance = (TAccountResource)await Repository.Modify(Instance).ConfigureAwait(false);
            return (TAccountEditor)this;
        }

        public Task<AccountUsageResource> Usages()
        {
            return Repository.Client.Get<AccountUsageResource>(Instance.Link("Usages"));
        }
    }
}