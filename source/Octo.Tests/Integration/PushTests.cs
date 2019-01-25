using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Extensibility;
using Serilog;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Integration
{
    public class PushTests : IntegrationTestBase
    {
        private static readonly byte[] _fileBytes = {45, 11, 0, 255, 4};

        public PushTests()
        {
            Get($"{TestRootPath}/api/users/me", p => Response.AsJson(
                new UserResource()
                {
                    Links = new LinkCollection()
                    {
                        {"Spaces", TestRootPath + "/api/users/users-1/spaces" }
                    }
                }
            ));

            Get($"{TestRootPath}/api/users/users-1/spaces", p => Response.AsJson(
                    new[] {
                        new SpaceResource() { Id = "Spaces-1", IsDefault = true},
                        new SpaceResource() { Id = "Spaces-2", IsDefault = false}
                    }
            ));


            Get($"{TestRootPath}/api/spaces-1", p => Response.AsJson(
                new SpaceRootResource()
            ));

            Post(TestRootPath + "/api/packages/raw", p =>
            {
                var file = Request.Files.First();
                using (var ms = new MemoryStream())
                {
                    file.Value.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    Log.Information("Package received {bytes}", string.Join(", ", ms.ToArray()));
                }
                return HttpStatusCode.OK;
            });
        }

        [Test]
        public void PushPackage()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempFile, _fileBytes);
                var result = Execute("push", $"--package={tempFile}");
                result.LogOutput.Should().Contain(@"Package received ""45, 11, 0, 255, 4""");
                result.Code.Should().Be(0);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}