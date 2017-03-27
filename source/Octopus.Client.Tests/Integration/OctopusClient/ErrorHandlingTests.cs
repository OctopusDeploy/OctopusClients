using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class ErrorHandlingTests : HttpIntegrationTestBase
    {
        public ErrorHandlingTests() : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Post(TestRootPath, p => Response.AsJson(new OctopusExceptionFactory.OctopusErrorsContract()
            {
                ErrorMessage = "ErrorMessage",
                Errors = new []{ "Error" }, 
                Details = new[] { "Details" }
            }, HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task ShouldHandleValidationError()
        {
            Func<Task> post = async () => { await AsyncClient.Post("~/"); };
            post.ShouldThrow<OctopusValidationException>()
                .And
                .DetailsAs<string[]>().Single().Should().Be("Details");
        }
    }
}
