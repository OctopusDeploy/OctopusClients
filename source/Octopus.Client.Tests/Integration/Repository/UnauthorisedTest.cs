using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.Repository
{
    public class UnauthorisedTest : HttpIntegrationTestBase
    {
        const string ErrorMessage = "You must be logged in to perform this action. Please provide a valid API key or log in again.";
        public UnauthorisedTest()
        {
            Get($"{TestRootPath}api/users/me", p =>
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
            var repo = new OctopusAsyncRepository(Client);
            Func<Task> getUser = () => repo.Users.GetCurrent();
            getUser.ShouldThrow<OctopusSecurityException>().WithMessage(ErrorMessage);
        }
    }
}