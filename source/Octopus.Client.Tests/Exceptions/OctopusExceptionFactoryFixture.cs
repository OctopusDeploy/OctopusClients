using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Exceptions
{
    public class OctopusExceptionFactoryFixture
    {
        [Test]
        [TestCase(HttpStatusCode.BadRequest)]
        [TestCase(HttpStatusCode.Conflict)]
        public async Task Http400or409Response_WithNoPayLoad_ShouldCreateOctopusValidationException(
            HttpStatusCode statusCode)
        {
            var httpResponseMessage = new HttpResponseMessage(statusCode);
            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage);

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusValidationException>();
                createdException.HttpStatusCode.Should().Be((int)statusCode);
            }
        }

        [Test]
        [TestCaseSource(typeof(ErrorPayloads), nameof(ErrorPayloads.ForValidationExceptions))]
        public async Task CreatedOctopusValidationException_ShouldContainPayloadInformation(string payload, OctopusValidationException expectedResult)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            if (!string.IsNullOrWhiteSpace(payload))
            {
                httpResponseMessage.Content = new StringContent(payload);
            }

            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusValidationException;
            createdException.Should().NotBeNull();

            using (new AssertionScope())
            {
                // ReSharper disable once PossibleNullReferenceException
                createdException.ErrorMessage.Should().Be(expectedResult.ErrorMessage);
                createdException.Errors.Should().BeEquivalentTo(expectedResult.Errors);
                createdException.HelpText.Should().Be(expectedResult.HelpText);
            }
        }

        [Test]
        [TestCase(HttpStatusCode.Unauthorized)]
        [TestCase(HttpStatusCode.Forbidden)]
        public async Task Http401or403Response_WithNoPayLoad_ShouldCreateOctopusSecurityException(
            HttpStatusCode statusCode)
        {
            var httpResponseMessage = new HttpResponseMessage(statusCode);
            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage);

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusSecurityException>();
                createdException.HttpStatusCode.Should().Be((int)statusCode);
            }
        }

        [Test]
        [TestCaseSource(typeof(ErrorPayloads), nameof(ErrorPayloads.ForSecurityExceptions))]
        public async Task CreatedOctopusSecurityException_ShouldContainPayloadInformation(string payload, OctopusSecurityException expectedResult)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            if (!string.IsNullOrWhiteSpace(payload))
            {
                httpResponseMessage.Content = new StringContent(payload);
            }

            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusSecurityException;
            createdException.Should().NotBeNull();

            using (new AssertionScope())
            {
                // ReSharper disable once PossibleNullReferenceException
                createdException.Message.Should().Be(expectedResult.Message);
                createdException.HelpText.Should().Be(expectedResult.HelpText);
            }
        }

        [Test]
        public async Task Http404Response_WithNoPayLoad_ShouldCreateOctopusResourceNotFoundException()
        {
            const HttpStatusCode statusCode = HttpStatusCode.NotFound;

            var httpResponseMessage = new HttpResponseMessage(statusCode);
            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage);

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusResourceNotFoundException>();
                createdException.HttpStatusCode.Should().Be((int)statusCode);
            }
        }

        [Test]
        [TestCaseSource(typeof(ErrorPayloads), nameof(ErrorPayloads.ForResourceNotFoundExceptions))]
        public async Task CreatedOctopusResourceNotFoundException_ShouldContainPayloadInformation(string payload, OctopusResourceNotFoundException expectedResult)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            if (!string.IsNullOrWhiteSpace(payload))
            {
                httpResponseMessage.Content = new StringContent(payload);
            }

            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusResourceNotFoundException;
            createdException.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            createdException.Message.Should().Be(expectedResult.Message);
        }

        [Test]
        public async Task Http405Response_WithNoPayLoad_ShouldCreateOctopusMethodNotAllowedFoundException()
        {
            const HttpStatusCode httpStatusCode = HttpStatusCode.MethodNotAllowed;

            var httpResponseMessage = new HttpResponseMessage(httpStatusCode);
            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage);

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusMethodNotAllowedFoundException>();
                createdException.HttpStatusCode.Should().Be((int)httpStatusCode);
            }
        }

        [Test]
        [TestCaseSource(typeof(ErrorPayloads), nameof(ErrorPayloads.ForMethodNotAllowedExceptions))]
        public async Task CreatedOctopusMethodNotAllowedFoundException_ShouldContainPayloadInformation(string payload, OctopusMethodNotAllowedFoundException expectedResult)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);

            if (!string.IsNullOrWhiteSpace(payload))
            {
                httpResponseMessage.Content = new StringContent(payload);
            }

            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusMethodNotAllowedFoundException;
            createdException.Should().NotBeNull();
            // ReSharper disable once PossibleNullReferenceException
            createdException.Message.Should().Be(expectedResult.Message);
        }

        [Test]
        [TestCase(HttpStatusCode.Ambiguous)]
        [TestCase(HttpStatusCode.MovedPermanently)]
        [TestCase(HttpStatusCode.Redirect)]
        [TestCase(HttpStatusCode.RedirectMethod)]
        [TestCase(HttpStatusCode.NotModified)]
        [TestCase(HttpStatusCode.UseProxy)]
        [TestCase(HttpStatusCode.Unused)]
        [TestCase(HttpStatusCode.TemporaryRedirect)]
        [TestCase(HttpStatusCode.PaymentRequired)]
        [TestCase(HttpStatusCode.NotAcceptable)]
        [TestCase(HttpStatusCode.ProxyAuthenticationRequired)]
        [TestCase(HttpStatusCode.RequestTimeout)]
        [TestCase(HttpStatusCode.Gone)]
        [TestCase(HttpStatusCode.LengthRequired)]
        [TestCase(HttpStatusCode.PreconditionFailed)]
        [TestCase(HttpStatusCode.RequestEntityTooLarge)]
        [TestCase(HttpStatusCode.RequestUriTooLong)]
        [TestCase(HttpStatusCode.UnsupportedMediaType)]
        [TestCase(HttpStatusCode.RequestedRangeNotSatisfiable)]
        [TestCase(HttpStatusCode.ExpectationFailed)]
        [TestCase(HttpStatusCode.UpgradeRequired)]
        public async Task
            HttpResponseWithStatusCodeExcluding_400_401_403_404_405_409_AndNoPayload_ShouldCreateOctopusServerException(
                HttpStatusCode statusCode)
        {
            var httpResponseMessage = new HttpResponseMessage(statusCode);
            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage);

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusServerException>();
                createdException.HttpStatusCode.Should().Be((int)statusCode);
            }
        }

        [Test]
        [TestCaseSource(typeof(ErrorPayloads), nameof(ErrorPayloads.ForServerExceptions))]
        public async Task CreatedOctopusServerException_ShouldContainPayloadInformation(string payload, OctopusServerException expectedResult)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            if (!string.IsNullOrWhiteSpace(payload))
            {
                httpResponseMessage.Content = new StringContent(payload);
            }

            var createdException = await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusServerException;
            createdException.Should().NotBeNull();

            using (new AssertionScope())
            {
                // Branching as Contain is not valid with empty string.
                // ReSharper disable once PossibleNullReferenceException
                if (string.IsNullOrWhiteSpace(createdException.Message))
                {
                    createdException.Message.Should().Be(expectedResult.Message);
                }
                else
                {
                    // Using contain as message could be altered when FullException is not empty.
                    createdException.Message.Should().Contain(expectedResult.Message);
                }

                createdException.HelpText.Should().Be(expectedResult.HelpText);
            }
        }

        [Test]
        [TestCase(HttpStatusCode.BadGateway)]
        [TestCase(HttpStatusCode.ServiceUnavailable)]
        [TestCase(HttpStatusCode.GatewayTimeout)]
        public async Task Http502or503or504Response_ShouldCreateOctopusServerUnavailableException(HttpStatusCode statusCode)
        {
            var createdException = await OctopusExceptionFactory.CreateException(ErrorResponses.With(statusCode));

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusServerUnavailableException>();
                createdException.HttpStatusCode.Should().Be((int)statusCode);
            }
        }

        [Test]
        public async Task OctopusServerUnavailableException_ShouldStillBeCaughtAsAnOctopusServerException()
        {
            var createdException = await OctopusExceptionFactory.CreateException(ErrorResponses.With(HttpStatusCode.ServiceUnavailable));

            createdException.Should().BeAssignableTo<OctopusServerException>();
        }

        [Test]
        public async Task AnHtmlMaintenancePage_ShouldBeReducedToItsReadableText()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, ErrorResponses.MaintenancePage, "text/html", "https://example.octopus.app/api/Spaces-1/tenants?skip=0"));

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusServerUnavailableException>();
                createdException.Message.Should().NotContain("<");
                createdException.Message.Should().NotContain("Unexpected json deserialization error");
                createdException.Message.Should().NotContain("font-family");
                createdException.Message.Should().NotContain("location.reload");
                createdException.Message.Should().Contain("Down for maintenance");
                // octopus-teamcity's OctopusErrorClassifier matches this text to classify the failure as transient.
                createdException.Message.Should().Contain("undergoing maintenance");
                createdException.Message.Should().Contain("maintenance & will be back soon");
                createdException.Message.Should().Contain("(text/html)");
                createdException.Message.Should().Contain("https://example.octopus.app");
                createdException.Message.Should().NotContain("/api/Spaces-1/tenants");
            }
        }

        [Test]
        public async Task AJsonErrorBody_ShouldProvideTheServersOwnErrorMessage()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, ErrorResponses.MaintenanceJson, "application/json"));

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusServerUnavailableException>();
                createdException.Message.Should().Be("Your Octopus Cloud instance is currently undergoing maintenance and will be back soon.");
            }
        }

        [Test]
        public async Task AnEmptyBody_ShouldDescribeTheResponseRatherThanSayNothing()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, requestUri: "https://example.octopus.app/api/Spaces-1/tenants"));

            createdException.Message.Should().Be("Octopus Server at https://example.octopus.app returned 503 ServiceUnavailable with an empty response body.");
        }

        [Test]
        [TestCase("text/plain")]
        [TestCase("application/problem+json")]
        [TestCase(null)]
        public async Task AnErrorContractIsStillReadWhenTheContentTypeDoesNotSayJson(string mediaType)
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.InternalServerError, "{\"ErrorMessage\":\"Something specific went wrong\"}", mediaType));

            createdException.Message.Should().Be("Something specific went wrong");
        }

        [Test]
        public async Task AMalformedJsonBody_ShouldBeDescribedRatherThanReportedAsADeserializationFailure()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.InternalServerError, "{\"ErrorMessage\": \"the response was cut off", "application/json"));

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusServerException>();
                createdException.Message.Should().NotContain("Unexpected json deserialization error");
                createdException.Message.Should().Contain("the response was cut off");
            }
        }

        [Test]
        public async Task AnHtmlPageLabelledAsJson_ShouldStillBeReducedToItsReadableText()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, ErrorResponses.MaintenancePage, "application/json"));

            using (new AssertionScope())
            {
                createdException.Message.Should().NotContain("<");
                createdException.Message.Should().Contain("undergoing maintenance");
            }
        }

        [Test]
        public async Task AJsonBodyWithNoErrorMessage_ShouldDescribeTheResponse()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.InternalServerError, "{\"Foo\":1}", "application/json"));

            using (new AssertionScope())
            {
                createdException.Message.Should().Contain("(application/json)");
                createdException.Message.Should().Contain("{\"Foo\":1}");
            }
        }

        [Test]
        public async Task AnHtmlBodyOnAStatusThatIsNotUnavailable_ShouldStillBeSummarised()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.NotFound, ErrorResponses.MaintenancePage, "text/html"));

            using (new AssertionScope())
            {
                createdException.Should().BeOfType<OctopusResourceNotFoundException>();
                createdException.Message.Should().NotContain("<");
                createdException.Message.Should().Contain("404 NotFound (text/html)");
            }
        }

        [Test]
        public async Task ALargeBody_ShouldBeTruncated()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, "<html><body><p>" + new string('x', 50000) + "</p></body></html>", "text/html"));

            createdException.Message.Length.Should().BeLessThan(700);
        }

        [Test]
        public async Task AMaintenancePageWithASelfClosingScript_ShouldKeepItsReadableText()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, ErrorResponses.MaintenancePageWithSelfClosingScript, "text/html"));

            using (new AssertionScope())
            {
                createdException.Message.Should().Contain("Down for maintenance");
                createdException.Message.Should().Contain("undergoing maintenance");
                createdException.Message.Should().NotContain("reload.js");
            }
        }

        [Test]
        public async Task EscapedMarkupInTheBody_ShouldNotReachTheMessageAsMarkup()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, ErrorResponses.PageWithEscapedMarkup, "text/html"));

            using (new AssertionScope())
            {
                createdException.Message.Should().NotContain("<");
                createdException.Message.Should().Contain("undergoing maintenance");
            }
        }

        [Test]
        public async Task CredentialsInTheRequestUri_ShouldNotReachTheMessage()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, requestUri: "https://svc:s3cret@example.octopus.app/api/tenants"));

            using (new AssertionScope())
            {
                createdException.Message.Should().NotContain("s3cret");
                createdException.Message.Should().NotContain("svc");
                createdException.Message.Should().Contain("https://example.octopus.app");
            }
        }

        [Test]
        public async Task ABodyWithNoReadableText_ShouldBeDescribedRatherThanQuotedAsEmpty()
        {
            var createdException = await OctopusExceptionFactory.CreateException(
                ErrorResponses.With(HttpStatusCode.ServiceUnavailable, ErrorResponses.PageWithNoReadableText, "text/html"));

            using (new AssertionScope())
            {
                createdException.Message.Should().NotEndWith(": \"\"");
                createdException.Message.Should().Contain("no readable content");
            }
        }

        [Test]
        public async Task AResponseWithNoContentTypeAtAll_ShouldStillReadTheErrorContract()
        {
            var response = ErrorResponses.With(HttpStatusCode.InternalServerError, "{\"ErrorMessage\":\"Something specific went wrong\"}");

            using (new AssertionScope())
            {
                response.Content.Headers.ContentType.Should().BeNull();
                var createdException = await OctopusExceptionFactory.CreateException(response);
                createdException.Message.Should().Be("Something specific went wrong");
            }
        }
    }
}
