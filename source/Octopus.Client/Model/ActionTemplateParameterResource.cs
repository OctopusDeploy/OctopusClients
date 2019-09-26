using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ActionTemplateParameterResource
    {
        public ActionTemplateParameterResource()
        {
            DisplaySettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Id { get; set; }

        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        [Trim]
        public string Label { get; set; }

        [Writeable]
        public string HelpText { get; set; }

        [Writeable]
        public PropertyValueResource DefaultValue { get; set; }

        [Writeable]
        public IDictionary<string, string> DisplaySettings { get; set; }

        public bool IsCertificate()
        {
            return IsControlType(ControlType.Certificate);
        }

        public bool IsSensitive()
        {
            return IsControlType(ControlType.Sensitive);
        }
        
        public bool IsAmazonWebServicesAccount()
        {
            return IsControlType(ControlType.AmazonWebServicesAccount);
        }
        
        public bool IsAzureAccount()
        {
            return IsControlType(ControlType.AzureAccount);
        }

        public string GetControlType()
        {
            return DefaultValue != null && DisplaySettings.ContainsKey(ControlType.ControlTypeKey) ? DisplaySettings[ControlType.ControlTypeKey] : null;
        }

        public bool IsControlType(string controlType)
        {
            return GetControlType() == controlType;
        }
    }
}