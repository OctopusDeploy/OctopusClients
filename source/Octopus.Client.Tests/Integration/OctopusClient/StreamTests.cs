using System;
using System.IO;
using FluentAssertions;
using Nancy;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class StreamTests : OctopusClientTestBase
    {
        private const string Path = "/StreamTests";
        private static bool _recieved;

        public StreamTests()
        {
            Post[Path] = p =>
            {
                if (Request.Headers.ContentType != "application/octet-stream")
                    return CreateErrorResponse($"Wrong header type, found '{Request.Headers.ContentType}'");

                if (!CompareStreamToSharedBytes(Request.Body))
                    return CreateErrorResponse($"Body does not match");

                _recieved = true;
                return HttpStatusCode.NoContent;
            };
        }

        [Test]
        public void PostStream()
        {
            using (var ms = new MemoryStream(SharedBytes))
            {
                _recieved = false;
                Action post = () => Client.Post(Path, ms);
                post.ShouldNotThrow();
                _recieved.Should().BeTrue();
            }
        }
    }
}