using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SmtpConfiguration
{
    public class GoogleSmtpConfigurationResource : SmtpCredentialDetailsResource
    {
        [Writeable]
        public string GoogleAudience { get; set; }
        
        [Writeable]
        public string GoogleServiceAccount { get; set; }
    
    }  
}