using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Serialization;
using Octopus.TinyTypes;

namespace Octopus.Client.Tests.Serialization
{
    public class TinyTypeSerializationFixture
    {
        [Test]
        public void SerializesResourceTypesWithTinyTypeProperties()
        {
            var json = JsonSerialization.SerializeObject(new Resource { Id = new MyId("blah") });
            json.Should().Be(@"{
  ""Id"": ""blah""
}");
        }

        [Test]
        public void DeserializesResourceTypesWithTinyTypeProperties()
        {
            var json = @"{""Id"": ""blah""}";
            var obj = JsonSerialization.DeserializeObject<Resource>(json);
            obj.Should().BeEquivalentTo(new Resource { Id = new MyId("blah") });
        }

        public class Resource
        {
            public MyId Id { get; set; }
        }

        public class MyId : CaseInsensitiveStringTinyType
        {
            public MyId(string value) : base(value)
            {
            }
        }
    }
}