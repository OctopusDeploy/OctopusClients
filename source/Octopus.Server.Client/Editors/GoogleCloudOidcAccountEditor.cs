using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class GoogleCloudOidcAccountEditor : AccountEditor<GoogleCloudOidcAccountResource, GoogleCloudOidcAccountEditor>
    {
        public GoogleCloudOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}
