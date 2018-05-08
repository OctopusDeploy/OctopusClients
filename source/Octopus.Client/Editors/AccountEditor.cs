using System;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AccountEditor<TAccountResource, TAccountEditor> : IResourceEditor<TAccountResource, TAccountEditor>
        where TAccountResource : AccountResource, new()
        where TAccountEditor : AccountEditor<TAccountResource, TAccountEditor>
    {
        private readonly IAccountRepository repository;

        public AccountEditor(IAccountRepository repository)
        {
            this.repository = repository;
        }

        public TAccountResource Instance { get; private set; }

        public virtual TAccountEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = (TAccountResource)repository.Create(new TAccountResource
                {
                    Name = name
                });
            }
            else
            {
                if (!(existing is TAccountResource))
                {
                    throw new ArgumentException($"An account with that name exists but is not of type {typeof(TAccountResource).Name}");
                }

                existing.Name = name;

                Instance = (TAccountResource)repository.Modify(existing);
            }

            return (TAccountEditor) this;
        }

        public virtual TAccountEditor Customize(Action<TAccountResource> customize)
        {
            customize?.Invoke(Instance);
            return (TAccountEditor)this;
        }

        public virtual TAccountEditor Save()
        {
            Instance = (TAccountResource)repository.Modify(Instance);
            return (TAccountEditor)this;
        }
    }
}