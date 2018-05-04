using System;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
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

        public async Task<AccountEditor<TAccountResource>> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name);
            if (existing == null)
            {
                Instance = (TAccountResource)await repository.Create(new TAccountResource
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

                Instance = (TAccountResource)await repository.Modify(existing);
            }

            return this;
        }

        public virtual AccountEditor<TAccountResource> Customize(Action<TAccountResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public virtual async Task<AccountEditor<TAccountResource>> Save()
        {
            Instance = (TAccountResource)await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}