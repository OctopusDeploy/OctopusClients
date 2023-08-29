using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AzureOidcAccountEditor : AccountEditor<AzureOidcAccountResource, AzureOidcAccountEditor>
    {
        public AzureOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}