using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AmazonWebServicesOidcAccountEditor : AccountEditor<AmazonWebServicesOidcAccountResource, AmazonWebServicesOidcAccountEditor>
    {
        public AmazonWebServicesOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}