using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Octopus.Client.Model
{
    public class ReleaseCreationStrategyResource
    {
        public ReleaseCreationStrategyResource()
        {
            additionalData = new Dictionary<string, JToken>();
        }
        
        public DeploymentActionPackageResource ReleaseCreationPackage { get; set; }

        public DeploymentActionPackageResource WorkItemPackage { get; set; }

        public string ChannelId { get; set; }
        
        #region Backward Compatibility
        
        /* Before support for multiple packages per deployment-action was added, ReleaseCreationStrategyResource contained
         * a property named 'ReleaseCreationPackageStepId' which contained the Id of the deployment action, as this was sufficient
         * to uniquely identify a package reference.
         * In 2018.9 the DeploymentActionPackageResource class was added to better represent this relationship, but
         * we still need to maintain backward-compatibility with older server versions. 
         */
        
        [JsonExtensionData]
        private IDictionary<string, JToken> additionalData;
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (additionalData.TryGetValue("ReleaseCreationPackageStepId", out var releaseCreationPackageStepIdToken) && 
                (ReleaseCreationPackage == null || string.IsNullOrEmpty(ReleaseCreationPackage.DeploymentAction)))
            {
                var legacyValue = releaseCreationPackageStepIdToken.Value<string>();

                if (string.IsNullOrEmpty(legacyValue))
                    return;
                
                ReleaseCreationPackage = DeploymentActionPackageResource.FromLegacyStringFormat(legacyValue);
            }
        }
        
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (ReleaseCreationPackage != null && !string.IsNullOrEmpty(ReleaseCreationPackage.DeploymentAction))
            {
                additionalData["ReleaseCreationPackageStepId"] = ReleaseCreationPackage.ToLegacyStringFormat();
            }
        }
        #endregion
    }
}