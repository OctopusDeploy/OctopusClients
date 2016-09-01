using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class FileTests : OctopusClientTestBase
    {
        private const string Path = "/FileTests";
        private static bool _recieved;

        public FileTests()
        {
            Post[Path] = p =>
            {
                var file = Request.Files.SingleOrDefault();
                if (file == null)
                    return CreateErrorResponse($"No file");

                if (!file.Name.StartsWith("foo.txt"))
                    return CreateErrorResponse($"Name does not start with 'foo.txt' found '{file.Name}'");

                if (!file.Key.StartsWith(@"file; filename=foo.txt"))
                    return CreateErrorResponse($"Key is not correct, found '{file.Key}'");

                if (file.ContentType != "application/octet-stream")
                    return CreateErrorResponse($"ContentType is not 'application/octet-stream' found '{file.ContentType}'");

                if (!CompareStreamToSharedBytes(file.Value))
                    return CreateErrorResponse($"Body does not match");

                _recieved = true;
                return HttpStatusCode.NoContent;
            };
        }

        [Test]
        public void PostFile()
        {
            using (var ms = new MemoryStream(SharedBytes))
            {
                var file = new FileUpload
                {
                    Contents = ms,
                    FileName = "foo.txt"
                };
                _recieved = false;
                Action post = () => Client.Post(Path, file);
                post.ShouldNotThrow();
                _recieved.Should().BeTrue();
            }
        }
    }
}