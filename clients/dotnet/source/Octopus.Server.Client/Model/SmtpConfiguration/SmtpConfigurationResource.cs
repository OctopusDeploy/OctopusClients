using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SmtpConfiguration
{
    public class SmtpConfigurationResource : Resource
    {
        [Writeable]
        public string SmtpHost { get; set; }

        [Writeable, Required]
        public int? SmtpPort { get; set; }

        [Writeable]
        public string SendEmailFrom { get; set; }

        [Writeable]
        public bool EnableSsl { get; set; }
        
        [Writeable]
        public int? Timeout { get; set; }
        
        [Writeable]
        public SmtpCredentialDetailsResource Details { get; set; }
    }
}