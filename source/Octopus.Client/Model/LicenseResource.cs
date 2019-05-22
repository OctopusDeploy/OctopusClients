using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class LicenseResource : Resource
    {
        [Writeable]
        public string LicenseText { get; set; }
        public string SerialNumber { get; set; }
    }
}