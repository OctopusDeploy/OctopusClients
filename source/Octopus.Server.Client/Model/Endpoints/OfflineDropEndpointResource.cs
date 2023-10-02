using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class OfflineDropEndpointResource : EndpointResource
    {
        public OfflineDropEndpointResource()
        {
            SensitiveVariablesEncryptionPassword = new SensitiveValue();
            additionalData = new Dictionary<string, JToken>();
        }

        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.OfflineDrop; }
        }
        
        [Writeable]
        public OfflineDropDestinationResource Destination { get; set; }

        [Writeable]
        public SensitiveValue SensitiveVariablesEncryptionPassword { get; set; }

        [Trim]
        [Writeable]
        public string ApplicationsDirectory { get; set; }

        [Trim]
        [Writeable]
        public string OctopusWorkingDirectory { get; set; }

        #region Backward Compatibility
        
       /* In 2018.9 offline-drop targets were modified to support writing to an Octopus artifact.
        * But we still need to support talking to older servers, where the DropFolderPath property
        * was directly on the OfflineDropEndpointResource.
        */

        [JsonExtensionData]
        private IDictionary<string, JToken> additionalData;
        
         [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (Destination != null && Destination.DestinationType == OfflineDropDestinationType.FileSystem)
            {
                additionalData["DropFolderPath"] = Destination.DropFolderPath;
                return;
            }

            additionalData["DropFolderPath"] = null;
        }
        
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (additionalData.TryGetValue("DropFolderPath", out var dropFolderPathToken) && Destination == null)
            {
                var legacyValue = dropFolderPathToken.Value<string>();

                if (string.IsNullOrEmpty(legacyValue))
                    return;

                Destination = new OfflineDropDestinationResource
                {
                    DestinationType = OfflineDropDestinationType.FileSystem,
                    DropFolderPath = legacyValue
                };
            }
        }
        
        #endregion
    }
}