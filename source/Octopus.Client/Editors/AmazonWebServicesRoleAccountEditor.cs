using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AmazonWebServicesRoleAccountEditor : AccountEditor<AmazonWebServicesRoleAccountResource>
    {
        public AmazonWebServicesRoleAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}