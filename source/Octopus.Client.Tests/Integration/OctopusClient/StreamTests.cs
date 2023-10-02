using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class StreamTests : HttpIntegrationTestBase
    {
        private static bool _received;

        public StreamTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Post(TestRootPath, p =>
            {
                if (Request.Headers.ContentType != "application/octet-stream")
                    return CreateErrorResponse($"Wrong header type, found '{Request.Headers.ContentType}'");

                if (!CompareStreamToSharedBytes(Request.Body))
                    return CreateErrorResponse($"Body does not match");

                _received = true;
                return HttpStatusCode.NoContent;
            });
        }

        [Test]
        public void PostStream()
        {
            using (var ms = new MemoryStream(SharedBytes))
            {
                _received = false;
                Func<Task> post = () => AsyncClient.Post("~/", ms);
                post.ShouldNotThrow();
                _received.Should().BeTrue();
            }
        }
    }
}