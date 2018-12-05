using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class ChannelVersionRuleResource : Resource
    {
        public ChannelVersionRuleResource()
        {
            additionalData = new Dictionary<string, JToken>();
            ActionPackages = new List<DeploymentActionPackageResource>();
        }
        
        [Writeable]
        [Trim]
        public string VersionRange { get; set; }

        [Writeable]
        [Trim]
        public string Tag { get; set; }
        
        [Writeable]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ICollection<DeploymentActionPackageResource> ActionPackages { get; set; }

        #region Backward Compatibility
        
        /* Before support for multiple packages per deployment-action was added, ChannelVersionRuleResource contained
         * a property named 'Actions' which contained the name of the deployment action, as this was sufficient
         * to uniquely identify a package reference.
         * Briefly (2018.8.0 - 2018.9) we were supporting multiple packages by combining the action name and the
         * package reference name into the string contained in the Actions collection.
         * In 2018.9 the DeploymentActionPackageResource class was added to better represent this relationship, but
         * we still need to maintain backward-compatibility with older server versions. 
         */
        
        [JsonExtensionData]
        private IDictionary<string, JToken> additionalData;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (additionalData.TryGetValue("Actions", out var actionsToken) && 
                !ActionPackages.Any() && 
                actionsToken is JArray actionsJArray && 
                actionsJArray.HasValues)
            {
                ActionPackages = actionsJArray.Values<string>()
                    .Select(DeploymentActionPackageResource.FromLegacyStringFormat)
                    .ToList();
            }
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
           additionalData["Actions"] = JArray.FromObject(
               ActionPackages
                   .Select(ap => ap.ToLegacyStringFormat())
                   .Distinct().ToList());
        }
        #endregion
    }
}