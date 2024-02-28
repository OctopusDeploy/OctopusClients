using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class TentacleConfigurationConverterFixture
    {
        [Test]
        public void ListeningTentacle()
        {
            var input = new
            {
                CommunicationMode = nameof(TentacleCommunicationModeResource.Listening),
                Uri = "The Uri",
                Thumbprint = "The thumbprint",
                ProxyId = "The ProxyId"
            };

            var result = Execute<ListeningTentacleEndpointConfigurationResource>(input);

            result.CommunicationMode.Should().Be(TentacleCommunicationModeResource.Listening);
            result.Uri.Should().Be(input.Uri);
            result.Thumbprint.Should().Be(input.Thumbprint);
            result.ProxyId.Should().Be(input.ProxyId);
        }

        [Test]
        public void PollingTentacle()
        {
            var input = new
            {
                CommunicationMode = nameof(TentacleCommunicationModeResource.Polling),
                Uri = "The Uri",
                Thumbprint = "The thumbprint",
            };

            var result = Execute<PollingTentacleEndpointConfigurationResource>(input);

            result.CommunicationMode.Should().Be(TentacleCommunicationModeResource.Polling);
            result.Uri.Should().Be(input.Uri);
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