using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AmazonWebServicesRoleAccountEditor : AccountEditor<AmazonWebServicesRoleAccountResource, AmazonWebServicesRoleAccountEditor>
    {
        public AmazonWebServicesRoleAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}