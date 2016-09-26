using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class FeedResourceConverterFixture
    {
        [Test]
        public void DockerFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "Blah",
                FeedType = FeedType.Docker,
                ApiVersion = "Cat"
            };

            var result = Execute<DockerFeedResource>(input);

            Assert.AreEqual(FeedType.Docker, result.FeedType);
            Assert.IsAssignableFrom(typeof(DockerFeedResource), result);
            Assert.AreEqual(input.ApiVersion, result.ApiVersion);
        }

        [Test]
        public void NuGetFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "Blah",
                FeedType = FeedType.NuGet
            };

            var result = Execute<FeedResource>(input);

            Assert.AreEqual(FeedType.NuGet, result.FeedType);
            Assert.IsAssignableFrom(typeof(NuGetFeedResource), result);
        }

        [Test]
        public void MissingFeedTypeDeserializesAsFeedNuGet()
        {
            var input = new
            {
                Name = "Blah",
            };

            var result = Execute<FeedResource>(input);

            Assert.AreEqual(FeedType.NuGet, result.FeedType);
            Assert.IsAssignableFrom(typeof(NuGetFeedResource), result);
        }

        private static T Execute<T>(object input)
        {
            var settings = JsonSerialization.GetDefaultSerializerSettings();
            var json = JsonConvert.SerializeObject(input, settings);
            return (T)new FeedConverter()
                .ReadJson(
                    new JsonTextReader(new StringReader(json)),
                    typeof(T),
                    null,
                    JsonSerializer.Create(settings)
                );
        }
    }
}
