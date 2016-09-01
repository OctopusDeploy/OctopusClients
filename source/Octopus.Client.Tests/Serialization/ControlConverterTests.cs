using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model.Forms;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    public class ControlConverterTests
    {
        [Test]
        public void CanDeserializeVariableValue()
        {
            var input = new
            {
                Name = "name",
                Label = "lbl",
                Type= "VariableValue",
                Description = "desc",
                IsSecure = true
            };

            var result = Execute<VariableValue>(input);
            result.Label.Should().Be(input.Label);
            result.Name.Should().Be(input.Name);
            result.Description.Should().Be(input.Description);
            result.IsSecure.Should().Be(input.IsSecure);
        }

        [Test]
        public void CanDeserializePreV3_1_6VariableValue()
        {
            var input = new
            {
                Label = "lbl",
                Type= "VariableValue",
                Description = "desc",
                IsSecure = true
            };

            var result = Execute<VariableValue>(input);
            result.Label.Should().Be(input.Label);
            result.Name.Should().Be(input.Label);
            result.Description.Should().Be(input.Description);
            result.IsSecure.Should().Be(input.IsSecure);
        }

        private static T Execute<T>(object input)
        {
            var settings = JsonSerialization.GetDefaultSerializerSettings();
            var json = JsonConvert.SerializeObject(input, settings);
            return (T)new ControlConverter()
                .ReadJson(
                    new JsonTextReader(new StringReader(json)),
                    typeof(T),
                    null,
                    JsonSerializer.Create(settings)
                );
        }
    }
}