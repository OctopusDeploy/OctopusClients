using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SmtpConfiguration;

public class SmtpCredentialDetailsResource
{
    [Writeable]
    public string CredentialType { get; set; }
}