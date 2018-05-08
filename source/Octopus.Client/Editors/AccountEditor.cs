using System;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Accounts.Usages;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
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

        public TAccountEditor CreateOrModify(string name)
        {
            var existing = Repository.FindByName(name);
            if (existing == null)
            {
                Instance = (TAccountResource) Repository.Create(new TAccountResource
                {
                    Name = name
                });
            }
            else
            {
                if (!(existing is TAccountResource))
                {
                    throw new ArgumentException(
                        $"An account with that name exists but is not of type {typeof(TAccountResource).Name}");
                }

                Instance = (TAccountResource) existing;
            }

            return (TAccountEditor) this;
        }
        
        public TAccountEditor FindByName(string name)
        {
            var existing = Repository.FindByName(name);
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

        public virtual TAccountEditor Save()
        {
            Instance = (TAccountResource)Repository.Modify(Instance);
            return (TAccountEditor)this;
        }

        public AccountUsageResource Usage()
        {
            return Repository.Client.Get<AccountUsageResource>(Instance.Link("Usage"));
        }
    }
}