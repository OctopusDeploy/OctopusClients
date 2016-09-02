using System;
using System.Diagnostics;
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

            Get($"{TestRootPath}api", p => Response.AsJson(
                 new RootResource()
                 {
                     ApiVersion = "3.0.0",
                     Links = new LinkCollection()
                     {
                         { "CurrentUser",$"{TestRootPath}/api/users/me" }
                     }
                 }
             ));
            Get($"{TestRootPath}api/users/me", p =>
            {
                var response = Response.AsJson(
                    new { ErrorMessage = ErrorMessage },
                    HttpStatusCode.Unauthorized
                );
                response.Headers["Server"] = "Octopus Deploy";
                response.Headers["Cache-Control"] = "no-cache";
                response.Headers["Expires"] = "Thu, 01 Sep 2016 02:31:46 GMT";
                response.Headers["X-UA-Compatible"] = "IE=edge";
                response.Headers["X-Frame-Options"] = "DENY";
                return response;
            });
        }

        [Test]
        public void IfTheServerReturnsAnUnauthorisedResultASecurityExceptionShouldBeThrown()
        {
            var repo = new OctopusRepository(Client);
            var root = repo.Client.RootDocument;
            Action getUser = () => repo.Users.GetCurrent();
            getUser.ShouldThrow<OctopusSecurityException>().WithMessage(ErrorMessage);
        }
    }
}