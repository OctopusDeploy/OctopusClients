using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class SshKeyPairAccountResource : AccountResource
    {
        public SshKeyPairAccountResource()
        {
            PrivateKeyFile = new SensitiveValue();
            PrivateKeyPassphrase = new SensitiveValue();
        }

        public override AccountType AccountType
        {
            get { return AccountType.SshKeyPair; }
        }

        [Trim]
        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue PrivateKeyFile { get; set; }

        [Writeable]
        public SensitiveValue PrivateKeyPassphrase { get; set; }
    }
}