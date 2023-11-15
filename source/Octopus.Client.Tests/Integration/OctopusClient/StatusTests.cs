using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class StatusTests : HttpIntegrationTestBase
    {
        public StatusTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get($"{TestRootPath}/401", p => Response.AsJson(
                new { ErrorMessage = "You must be logged in to perform this action. Please provide a valid API key or log in again." },
                HttpStatusCode.Unauthorized
            ));

            Get(TestRootPath + "/{code:int}", p =>
            {
                var response = CreateErrorResponse($"Status {(int) p.code} as requested");
                response.StatusCode = (HttpStatusCode) (int) p.code;
                return response;
            });
        }

        [Test]
        public async Task Status400()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/400");
            await get.Should().ThrowAsync<OctopusValidationException>();
        }

        [Test]
        public async Task Status401()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/401");
            await get.Should().ThrowAsync<OctopusSecurityException>();
        }

        [Test]
        public async Task Status403()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/403");
            await get.Should().ThrowAsync<OctopusSecurityException>();
        }

        [Test]
        public async Task Status404()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/404");
            await get.Should().ThrowAsync<OctopusResourceNotFoundException>();
        }

        [Test]
        public async Task Status405()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/405");
            await get.Should().ThrowAsync<OctopusMethodNotAllowedFoundException>();
        }

        [Test]
        public async Task Status409()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/409");
            await get.Should().ThrowAsync<OctopusValidationException>();
        }

        [Test]
        public async Task Status500()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/500");
            await get.Should().ThrowAsync<OctopusServerException>();
        }
    }
}