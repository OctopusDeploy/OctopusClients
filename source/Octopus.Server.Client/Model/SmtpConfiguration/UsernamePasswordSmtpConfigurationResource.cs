using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SmtpConfiguration
{
    public class UsernamePasswordSmtpConfigurationResource: SmtpConfigurationResource
    {
        [Writeable]
        public string SmtpLogin { get; set; }
        
        [Writeable]
        public SensitiveValue SmtpPassword { get; set; }
        
        [NotReadable]
        [Writeable]
        [Obsolete("Use 'SmtpPassword' instead. Will be removed in version 5.0.0.", false)]
        public string NewSmtpPassword { get; set; }
    }  
}