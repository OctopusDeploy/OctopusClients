using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Octopus.Client.Model
{
    public class VersioningStrategyResource
    {
        public VersioningStrategyResource()
        {
            additionalData = new Dictionary<string, JToken>();
        }
        
        public string Template { get; set; }
        
        public DeploymentActionPackageResource DonorPackage { get; set; }
        
        #region Backward Compatibility
        
        /* Before support for multiple packages per deployment-action was added, VersioningStrategyResource contained
         * a property named 'DonorPackageStepId' which contained the Id of the deployment action, as this was sufficient
         * to uniquely identify a package reference.
         * In 2018.9 the DeploymentActionPackageResource class was added to better represent this relationship, but
         * we still need to maintain backward-compatibility with older server versions. 
         */
        
        [JsonExtensionData]
        private IDictionary<string, JToken> additionalData;
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (additionalData.TryGetValue("DonorPackageStepId", out var donorPackageStepIdToken) && 
                (DonorPackage == null || string.IsNullOrEmpty(DonorPackage.DeploymentAction)))
            {
                var legacyValue = donorPackageStepIdToken.Value<string>();

                if (string.IsNullOrEmpty(legacyValue))
                    return;
                
                DonorPackage = DeploymentActionPackageResource.FromLegacyStringFormat(legacyValue);
            }
        }
        
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (DonorPackage != null && !string.IsNullOrEmpty(DonorPackage.DeploymentAction))
            {
                additionalData["DonorPackageStepId"] = DonorPackage.ToLegacyStringFormat();
            }
        }
        
        #endregion 
    }
}