using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class GenericOidcAccountEditor : AccountEditor<GenericOidcAccountResource, GenericOidcAccountEditor>
    {
        public GenericOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}