using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AmazonWebServicesOidcAccountEditor : AccountEditor<AmazonWebServicesOidcAccountResource, AmazonWebServicesOidcAccountEditor>
    {
        public AmazonWebServicesOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}