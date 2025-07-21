using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class GenericOidcAccountEditor : AccountEditor<GenericOidcAccountResource, GenericOidcAccountEditor>
    {
        public GenericOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}