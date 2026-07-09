using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class GoogleCloudOidcAccountEditor : AccountEditor<GoogleCloudOidcAccountResource, GoogleCloudOidcAccountEditor>
    {
        public GoogleCloudOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}
