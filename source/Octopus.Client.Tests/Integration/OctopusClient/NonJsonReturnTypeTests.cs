using System.IO;
using System.Text;
using FluentAssertions;
using Nancy;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class NonJsonReturnTypeTests : HttpIntegrationTestBase
    {
        public NonJsonReturnTypeTests()
        {
            Get(TestRootPath, p => Response.AsJson(new TestDto() {Value = "42"}));
        }

        [Test]
        public void GetStream()
        {
            using (var stream = Client.Get<Stream>("~/"))
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
            var bytes = Client.Get<byte[]>("~/");
            var json = Encoding.UTF8.GetString(bytes);
            JsonConvert.DeserializeObject<TestDto>(json)
                .Value
                .Should()
                .Be("42");
        }

        [Test]
        public void GetContent()
        {
            using (var s = Client.GetContent("~/"))
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                content.Should().Be("{\r\n  \"Value\": \"42\"\r\n}");
            }
        }

        class TestDto
        {
            public string Value { get; set; }
        }
    }
}