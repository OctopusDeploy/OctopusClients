using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class UsernamePasswordAccountEditor : AccountEditor<UsernamePasswordAccountResource>
    {
        public UsernamePasswordAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}