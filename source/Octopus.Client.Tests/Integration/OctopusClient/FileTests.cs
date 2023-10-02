using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class FileTests : HttpIntegrationTestBase
    {
        private static bool _received;

        public FileTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Post(TestRootPath, p =>
            { 
                using (var reader = new StreamReader(Request.Body))
                {
                    var content = reader.ReadToEnd();
                    if (!content.Trim().EndsWith("--"))
                    {
                        return CreateErrorResponse(
                            $"The request did not include an appropriate boundary trailer ending with --");
                    }
                }
                
                var file = Request.Files.SingleOrDefault();
                if (file == null)
                    return CreateErrorResponse($"No file");

                if (!file.Name.StartsWith("foo.txt"))
                    return CreateErrorResponse($"Name does not start with 'foo.txt' found '{file.Name}'");

                if (!file.Key.StartsWith(@"file; filename=foo.txt"))
                    return CreateErrorResponse($"Key is not correct, found '{file.Key}'");

                if (file.ContentType != "application/octet-stream")
                    return
                        CreateErrorResponse(
                            $"ContentType is not 'application/octet-stream' found '{file.ContentType}'");

                if (!CompareStreamToSharedBytes(file.Value))
                    return CreateErrorResponse($"Body does not match");

                _received = true;
                return HttpStatusCode.NoContent;
            });
        }

        [Test]
        public async Task PostFile()
        {
            using (var ms = new MemoryStream(SharedBytes))
            {
                var file = new FileUpload
                {
                    Contents = ms,
                    FileName = "foo.txt"
                };
                _received = false;
                await AsyncClient.Post("~/", file).ConfigureAwait(false);
                _received.Should().BeTrue();
            }
        }

        [Test]
        public void PostFileSync()
        {
            using (var ms = new MemoryStream(SharedBytes))
            {
                var file = new FileUpload
                {
                    Contents = ms,
                    FileName = "foo.txt"
                };
                _received = false;
                SyncClient.Post("~/", file);
                _received.Should().BeTrue();
            }
        }
    }
}