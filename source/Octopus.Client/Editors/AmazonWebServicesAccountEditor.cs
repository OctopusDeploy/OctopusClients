using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AmazonWebServicesAccountEditor : AccountEditor<AmazonWebServicesAccountResource, AmazonWebServicesAccountEditor>
    {
        public AmazonWebServicesAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}