using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Tests.Extensions;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class NonJsonReturnTypeTests : HttpIntegrationTestBase
    {
        public NonJsonReturnTypeTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p => Response.AsJson(new TestDto() {Value = "42"}));
        }

        [Test]
        public async Task GetStream()
        {
            using (var stream = await AsyncClient.Get<Stream>("~/").ConfigureAwait(false))
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
        public async Task GetByteArray()
        {
            var bytes = await AsyncClient.Get<byte[]>("~/").ConfigureAwait(false);
            var json = Encoding.UTF8.GetString(bytes);
            JsonConvert.DeserializeObject<TestDto>(json)
                .Value
                .Should()
                .Be("42");
        }

        [Test]
        public async Task GetContent()
        {
            using (var s = await AsyncClient.GetContent("~/").ConfigureAwait(false))
            using (var ms = new MemoryStream())
            {
                s.CopyTo(ms);
                var content = Encoding.UTF8.GetString(ms.ToArray());
                content.RemoveNewlines().Should().Be("{  \"Value\": \"42\"}");
            }
        }

        class TestDto
        {
            public string Value { get; set; }
        }
    }
}