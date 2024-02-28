using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class EndpointConverterFixture
    {
        [Test]
        public void AzureWebApp()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.AzureWebApp),
                WebAppName = "The thumbprint"
            };

            var result = Execute<AzureWebAppEndpointResource>(input);

            result.WebAppName.Should().Be(input.WebAppName);
        }

        [Test]
        public void CloudRegion()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.None),
                DefaultWorkerPoolId = "The pool"
            };

            var result = Execute<CloudRegionEndpointResource>(input);

            result.DefaultWorkerPoolId.Should().Be(input.DefaultWorkerPoolId);
        }
        
        [Test]
        public void AzureCloudService()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.AzureCloudService),
                AccountId = "The account"
            };

            var result = Execute<CloudServiceEndpointResource>(input);

            result.AccountId.Should().Be(input.AccountId);
        }
        
        [Test]
        public void KubernetesApi()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.Kubernetes),
                ClusterUrl = "The URL"
            };

            var result = Execute<KubernetesEndpointResource>(input);

            result.ClusterUrl.Should().Be(input.ClusterUrl);
        }

        [Test]
        public void KUbernetesTentacle()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.KubernetesTentacle),
                TentacleEndpointConfiguration = new
                {
                    CommunicationMode = nameof(TentacleCommunicationModeResource.Listening),
                    Thumbprint = "The thumbprint",
                    Uri = "Some Uri"
                }
            };

            var result = Execute<KubernetesTentacleEndpointResource>(input);

            result.CommunicationStyle.Should().Be(CommunicationStyle.KubernetesTentacle);
            result.TentacleEndpointConfiguration.CommunicationMode.Should().Be(TentacleCommunicationModeResource.Listening);
        }

        [Test]
        public void OfflineDrop()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.OfflineDrop),
                Destination = new
                {
                    DestinationType = "FileSystem",
                    DropFolderPath = "The path"
                }
            };

            var result = Execute<OfflineDropEndpointResource>(input);

            result.Destination.DestinationType.Should().Be(OfflineDropDestinationType.FileSystem);
            result.Destination.DropFolderPath.Should().Be(input.Destination.DropFolderPath);
        }

        [Test]
        public void AzureServiceFabric()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.AzureServiceFabricCluster),
                SecurityMode = "SecureClientCertificate"
            };

            var result = Execute<ServiceFabricEndpointResource>(input);

            result.SecurityMode.Should().Be(AzureServiceFabricSecurityMode.SecureClientCertificate);
        }

        [Test]
        public void Ssh()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.Ssh),
                AccountId = "The account"
            };

            var result = Execute<SshEndpointResource>(input);

            result.AccountId.Should().Be(input.AccountId);
        }

        [Test]
        public void StepPackage()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.StepPackage),
                StepPackageVersion = "The version"
            };

            var result = Execute<StepPackageEndpointResource>(input);

            result.StepPackageVersion.Should().Be(input.StepPackageVersion);
        }

        [Test]
        public void ListeningTentacle()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.TentaclePassive),
                ProxyId = "The proxy",
                Thumbprint = "The thumb"
            };

            var result = Execute<ListeningTentacleEndpointResource>(input);

            result.Thumbprint.Should().Be(input.Thumbprint);
            result.ProxyId.Should().Be(input.ProxyId);
        }

        [Test]
        public void PollingTentacle()
        {
            var input = new
            {
                CommunicationStyle = nameof(CommunicationStyle.TentacleActive),
                Thumbprint = "The thumb"
            };

            var result = Execute<PollingTentacleEndpointResource>(input);

            result.Thumbprint.Should().Be(input.Thumbprint);
        }
        
        private static T Execute<T>(object input)
        {
            //Serialize anonymous object to JSON
            var json = JsonConvert.SerializeObject(input);
            
            var settings = JsonSerialization.GetDefaultSerializerSettings();
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
