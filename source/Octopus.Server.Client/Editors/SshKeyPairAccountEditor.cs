using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class SshKeyPairAccountEditor : AccountEditor<SshKeyPairAccountResource, SshKeyPairAccountEditor>
    {
        public SshKeyPairAccountEditor(IAccountRepository repository) : base(repository)
        {
        }
    }
}