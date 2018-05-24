using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class RootResource : Resource
    {
        public string Application { get; set; }
        public string Version { get; set; }
        public string ApiVersion { get; set; }
        public Guid InstallationId { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsEarlyAccessProgram { get; set; }
    }
}