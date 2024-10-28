﻿using System;
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
        public void MavenFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "Blah",
                FeedType = FeedType.Maven,
                DownloadAttempts = 91
            };

            var result = Execute<MavenFeedResource>(input);

            Assert.AreEqual(FeedType.Maven, result.FeedType);
            Assert.IsAssignableFrom(typeof(MavenFeedResource), result);
            Assert.AreEqual(input.DownloadAttempts, result.DownloadAttempts);
        }

        [Test]
        public void BuiltInFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "Blah",
                FeedType = FeedType.BuiltIn,
                DeleteUnreleasedPackagesAfterDays = 4
            };

            var result = Execute<BuiltInFeedResource>(input);

            Assert.AreEqual(FeedType.BuiltIn, result.FeedType);
            Assert.IsAssignableFrom(typeof(BuiltInFeedResource), result);
            Assert.AreEqual(input.DeleteUnreleasedPackagesAfterDays, result.DeleteUnreleasedPackagesAfterDays);
        }
        
        [Test]
        public void BuiltInArtifactFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "Blah",
                FeedType = FeedType.Artifact,
            };

            var result = Execute<BuiltInArtifactFeedResource>(input);

            Assert.AreEqual(FeedType.Artifact, result.FeedType);
            Assert.IsAssignableFrom(typeof(BuiltInArtifactFeedResource), result);
        }

        [Test]
        public void GitHubFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "GitIt",
                FeedType = FeedType.GitHub,
                DownloadAttempts = 91
            };

            var result = Execute<GitHubFeedResource>(input);

            Assert.AreEqual(FeedType.GitHub, result.FeedType);
            Assert.IsAssignableFrom(typeof(GitHubFeedResource), result);
            Assert.AreEqual(input.DownloadAttempts, result.DownloadAttempts);
        }
        
        
        [Test]
        public void ArtifactoryGenericFeedTypesDeserialize()
        {
            var input = new
            {
                Name = "Blah",
                FeedType = FeedType.ArtifactoryGeneric,
                DownloadAttempts = 91,
                FeedUri = "https://hello",
                Repository = "repo",
                LayoutRegex = "<SomeRegex>"
            };

            var result = Execute<ArtifactoryGenericFeedResource>(input);

            Assert.AreEqual(FeedType.ArtifactoryGeneric, result.FeedType);
            Assert.IsAssignableFrom(typeof(ArtifactoryGenericFeedResource), result);
            Assert.AreEqual(input.FeedUri, result.FeedUri);
            Assert.AreEqual(input.Repository, result.Repository);
            Assert.AreEqual(input.LayoutRegex, result.LayoutRegex);
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
