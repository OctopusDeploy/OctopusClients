using System;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AccountEditor<TAccountResource> : IResourceEditor<TAccountResource, AccountEditor<TAccountResource>>
        where TAccountResource : AccountResource, new()
    {
        private readonly IAccountRepository repository;

        public AccountEditor(IAccountRepository repository)
        {
            this.repository = repository;
        }

        public TAccountResource Instance { get; private set; }

        public AccountEditor<TAccountResource> CreateOrModify(string name)
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

            return this;
        }

        public virtual AccountEditor<TAccountResource> Customize(Action<TAccountResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public virtual AccountEditor<TAccountResource> Save()
        {
            Instance = (TAccountResource)repository.Modify(Instance);
            return this;
        }
    }
}