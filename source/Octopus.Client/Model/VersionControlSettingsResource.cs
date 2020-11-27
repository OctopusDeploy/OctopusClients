using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class VersionControlSettingsResource
    {
        [Writeable]
        public string Url { get; set; }
        [Writeable]
        public string Username { get; set; }
        [Writeable]
        public SensitiveValue Password { get; set; } = new SensitiveValue();
        [Writeable]
        public string DefaultBranch { get; set; }
        [Writeable]
        public string BasePath { get; set; }
    }
}