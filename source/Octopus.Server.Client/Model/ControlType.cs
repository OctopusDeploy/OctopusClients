using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public static class ControlType
    {
        public static readonly string ControlTypeKey = "Octopus.ControlType";

        public const string SingleLineText = "SingleLineText";
        public const string MultiLineText = "MultiLineText";
        public const string Select = "Select";
        public const string Checkbox = "Checkbox";
        public const string Sensitive = "Sensitive";
        public const string StepName = "StepName";
        public const string AzureAccount = "AzureAccount";
        public const string AmazonWebServicesAccount = "AmazonWebServicesAccount";
        public const string Certificate = "Certificate";
        public const string UsernamePasswordAccount = "UsernamePasswordAccount";

        public static Dictionary<string, string> AsDisplaySettings(string controlType)
        {
            return new Dictionary<string, string>()
            {
                {ControlTypeKey, controlType}
            };
        }
    }
}