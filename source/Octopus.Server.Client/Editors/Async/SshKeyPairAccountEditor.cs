using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class SshKeyPairAccountEditor : AccountEditor<SshKeyPairAccountResource, SshKeyPairAccountEditor>
    {
        public SshKeyPairAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}