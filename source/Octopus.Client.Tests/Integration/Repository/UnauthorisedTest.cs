using System;
using System.Collections;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Integration.Repository
{
    public class UnauthorisedTest : HttpIntegrationTestBase
    {
        const string ErrorMessage = "You must be logged in to perform this action. Please provide a valid API key or log in again.";
        public UnauthorisedTest()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get($"{TestRootPath}/api/users/users-1", p =>
            {
                var response = Response.AsJson(
                    new { ErrorMessage },
                    HttpStatusCode.Unauthorized
                );
                return response;
            });
        }

        [Test]
        public void IfTheServerReturnsAnUnauthorisedResultASecurityExceptionShouldBeThrown()
        {
            var repo = OctopusAsyncRepository.Create(AsyncClient).Result;
            Func<Task> getUser = () => repo.Users.Get("users-1");
            getUser.ShouldThrow<OctopusSecurityException>().WithMessage(ErrorMessage);
        }
    }
}