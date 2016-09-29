using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization
{
    /// <summary>
    /// Serializes <see cref="EndpointResource" />s by including and the CommunicationStyle property.
    /// </summary>
    public class EndpointConverter : InheritedClassConverter<EndpointResource, CommunicationStyle>
    {
        static readonly IDictionary<CommunicationStyle, Type> EndpointTypes =
          new Dictionary<CommunicationStyle, Type>
          {
                {CommunicationStyle.TentacleActive, typeof (PollingTentacleEndpointResource)},
                {CommunicationStyle.TentaclePassive, typeof (ListeningTentacleEndpointResource)},
                {CommunicationStyle.Ssh, typeof (SshEndpointResource)},
                {CommunicationStyle.OfflineDrop, typeof (OfflineDropEndpointResource)},
                {CommunicationStyle.AzureCloudService, typeof (CloudServiceEndpointResource)},
                {CommunicationStyle.AzureWebApp, typeof (AzureWebAppEndpointResource)},
                {CommunicationStyle.None, typeof (CloudRegionEndpointResource)}
          };

        protected override IDictionary<CommunicationStyle, Type> DerivedTypeMappings => EndpointTypes;
        protected override string TypeDesignatingPropertyName => "CommunicationStyle";
    }
}