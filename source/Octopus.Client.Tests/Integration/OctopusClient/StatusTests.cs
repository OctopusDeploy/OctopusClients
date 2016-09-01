using System;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class StatusTests : OctopusClientTestBase
    {
        private const string Path = "/StatusTests/";
        public StatusTests()
        {
            Get[Path + "{code:int}"] = p =>
            {
                var response = CreateErrorResponse($"Status {(int) p.code} as requested");
                response.StatusCode = (HttpStatusCode) (int) p.code;
                return response;
            };
        }

        [Test]
        public void Status400()
        {
            Action get = () => Client.Get<object>($"{Path}400");
            get.ShouldThrow<OctopusValidationException>();
        }

        [Test]
        public void Status401()
        {
            Action get = () => Client.Get<object>($"{Path}401");
            get.ShouldThrow<OctopusSecurityException>();
        }

        [Test]
        public void Status403()
        {
            Action get = () => Client.Get<object>($"{Path}403");
            get.ShouldThrow<OctopusSecurityException>();
        }

        [Test]
        public void Status404()
        {
            Action get = () => Client.Get<object>($"{Path}404");
            get.ShouldThrow<OctopusResourceNotFoundException>();
        }

        [Test]
        public void Status405()
        {
            Action get = () => Client.Get<object>($"{Path}405");
            get.ShouldThrow<OctopusMethodNotAllowedFoundException>();
        }

        [Test]
        public void Status409()
        {
            Action get = () => Client.Get<object>($"{Path}409");
            get.ShouldThrow<OctopusValidationException>();
        }

        [Test]
        public void Status500()
        {
            Action get = () => Client.Get<object>($"{Path}500");
            get.ShouldThrow<OctopusServerException>();
        }
    }
}