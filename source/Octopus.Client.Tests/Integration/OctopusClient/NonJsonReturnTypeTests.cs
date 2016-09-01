using System.IO;
using System.Text;
using FluentAssertions;
using Nancy;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class NonJsonReturnTypeTests : OctopusClientTestBase
    {
        private const string Path = "/NonJsonReturnTypeTests";

        public NonJsonReturnTypeTests()
        {
            Get[Path] = p => Response.AsJson(new TestDto() {Value = "42"});
        }

        [Test]
        public void GetStream()
        {
            using (var stream = Client.Get<Stream>(Path))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                new JsonSerializer()
                    .Deserialize<TestDto>(jsonReader)
                    .Value
                    .Should()
                    .Be("42");
            }
        }

        [Test]
        public void GetByteArray()
        {
            var bytes = Client.Get<byte[]>(Path);
            var json = Encoding.UTF8.GetString(bytes);
            JsonConvert.DeserializeObject<TestDto>(json)
                .Value
                .Should()
                .Be("42");
        }

        [Test]
        public void GetContent()
        {
            using (var s = Client.GetContent(Path))
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                content.Should().Be("{\"value\":\"42\"}");
            }
        }

        class TestDto
        {
            public string Value { get; set; }
        }
    }
}