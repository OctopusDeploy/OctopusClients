using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SmtpConfiguration
{
    public class AzureSmtpConfigurationResource : SmtpConfigurationResource
    {
        [Writeable]
        public string AzureAudience { get; set; }
        
        [Writeable]
        public string AzureClientId { get; set; }
        
        [Writeable]
        public string AzureTenantId { get; set; }
    
    }  
}