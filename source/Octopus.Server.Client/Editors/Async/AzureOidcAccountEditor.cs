using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AzureOidcAccountEditor : AccountEditor<AzureSubscriptionAccountResource, AzureOidcAccountEditor>
    {
        public AzureOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}