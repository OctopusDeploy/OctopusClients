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
        public void Status400()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/400");
            get.ShouldThrow<OctopusValidationException>();
        }

        [Test]
        public void Status401()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/401");
            get.ShouldThrow<OctopusSecurityException>();
        }

        [Test]
        public void Status403()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/403");
            get.ShouldThrow<OctopusSecurityException>();
        }

        [Test]
        public void Status404()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/404");
            get.ShouldThrow<OctopusResourceNotFoundException>();
        }

        [Test]
        public void Status405()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/405");
            get.ShouldThrow<OctopusMethodNotAllowedFoundException>();
        }

        [Test]
        public void Status409()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/409");
            get.ShouldThrow<OctopusValidationException>();
        }

        [Test]
        public void Status500()
        {
            Func<Task> get = () => AsyncClient.Get<object>("~/500");
            get.ShouldThrow<OctopusServerException>();
        }
    }
}