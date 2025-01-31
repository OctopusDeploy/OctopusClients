using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
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
        public string SmtpLogin { get; set; }

        [Writeable]
        public bool EnableSsl { get; set; }
        
        [Writeable]
        public int? Timeout { get; set; }

        [Writeable]
        public SensitiveValue SmtpPassword { get; set; }
        
        [Writeable]
        public string GoogleAudience { get; set; }
        
        [Writeable]
        public string GoogleServiceAccount { get; set; }

        [NotReadable]
        [Writeable]
        [Obsolete("Use 'SmtpPassword' instead. Will be removed in version 5.0.0.", false)]
        public string NewSmtpPassword { get; set; }
    }
}