using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AmazonWebServicesAccountEditor : AccountEditor<AmazonWebServicesAccountResource, AmazonWebServicesAccountEditor>
    {
        public AmazonWebServicesAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}