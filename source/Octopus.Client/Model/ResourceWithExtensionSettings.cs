using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public abstract class ResourceWithExtensionSettings : Resource
    {
        protected ResourceWithExtensionSettings()
        {
            ExtensionSettings = new List<ExtensionSettingsValues>();
        }

        [Writeable]
        public List<ExtensionSettingsValues> ExtensionSettings { get; set; }

        public TSettings GetExtensionSettings<TSettings>(string extensionId)
        {
            var settings = ExtensionSettings.SingleOrDefault(x => x.ExtensionId == extensionId);
            if (settings == null)
                return default(TSettings);

            var instance = JsonConvert.DeserializeObject<TSettings>(settings.Values.ToString());
            return instance;
        }
    }
}