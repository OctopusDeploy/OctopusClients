using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
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
        [TestCaseSource(nameof(CreatedOctopusExceptionTestData))]
        public async Task CreatedOctopusValidationException_ShouldContainPayloadInformation(
            OctopusExceptionFactory.OctopusErrorsContract octopusErrorsContract)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

            OctopusExceptionFactory.OctopusErrorsContract resultValidationObject = null;
            if (HasAnyData(octopusErrorsContract))
            {
                var content = JsonConvert.SerializeObject(octopusErrorsContract);
                httpResponseMessage.Content = new StringContent(content);
                resultValidationObject = OctopusExceptionFactory.OctopusErrorsContractFromBody(content);
            }

            var createdException =
                await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusValidationException;

            using (new AssertionScope())
            {
                createdException.Should().NotBeNull();

                if (resultValidationObject != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    createdException.ErrorMessage.Should().Be(resultValidationObject.ErrorMessage);
                    createdException.Errors.Should().BeEquivalentTo(resultValidationObject.Errors);
                    createdException.HelpText.Should().Be(resultValidationObject.HelpText);
                }
            }
        }

        public static IEnumerable<TestCaseData> CreatedOctopusExceptionTestData()
        {
            yield return new TestCaseData(new OctopusExceptionFactory.OctopusErrorsContract { });
            yield return new TestCaseData(
                new OctopusExceptionFactory.OctopusErrorsContract { ErrorMessage = "Error message" });
            yield return new TestCaseData(
                new OctopusExceptionFactory.OctopusErrorsContract
                    { ErrorMessage = "Error message", Errors = new[] { "Additional error" } });
            yield return new TestCaseData(
                new OctopusExceptionFactory.OctopusErrorsContract
                {
                    ErrorMessage = "Error message", Errors = new[] { "Additional error" }, HelpText = "Help text"
                });
            yield return new TestCaseData(
                new OctopusExceptionFactory.OctopusErrorsContract
                {
                    ErrorMessage = "Error message", Errors = new[] { "Additional error" }, HelpText = "Help text",
                    FullException = "Full exception"
                });
            yield return new TestCaseData(
                new OctopusExceptionFactory.OctopusErrorsContract
                    { Errors = new[] { "Additional error" }, HelpText = "Help text" });
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
        [TestCaseSource(nameof(CreatedOctopusExceptionTestData))]
        public async Task CreatedOctopusSecurityException_ShouldContainPayloadInformation(
            OctopusExceptionFactory.OctopusErrorsContract octopusErrorsContract)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            OctopusExceptionFactory.OctopusErrorsContract resultValidationObject = null;
            if (HasAnyData(octopusErrorsContract))
            {
                var content = JsonConvert.SerializeObject(octopusErrorsContract);
                httpResponseMessage.Content = new StringContent(content);
                resultValidationObject = OctopusExceptionFactory.OctopusErrorsContractFromBody(content);
            }

            var createdException =
                await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusSecurityException;

            using (new AssertionScope())
            {
                createdException.Should().NotBeNull();

                if (resultValidationObject != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    createdException.Message.Should().Be(resultValidationObject.ErrorMessage);
                    createdException.HelpText.Should().Be(resultValidationObject.HelpText);
                }
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
        [TestCaseSource(nameof(CreatedOctopusExceptionTestData))]
        public async Task CreatedOctopusResourceNotFoundException_ShouldContainPayloadInformation(
            OctopusExceptionFactory.OctopusErrorsContract octopusErrorsContract)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);

            OctopusExceptionFactory.OctopusErrorsContract resultValidationObject = null;
            if (HasAnyData(octopusErrorsContract))
            {
                var content = JsonConvert.SerializeObject(octopusErrorsContract);
                httpResponseMessage.Content = new StringContent(content);
                resultValidationObject = OctopusExceptionFactory.OctopusErrorsContractFromBody(content);
            }

            var createdException =
                await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusResourceNotFoundException;

            using (new AssertionScope())
            {
                createdException.Should().NotBeNull();

                if (resultValidationObject != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    createdException.Message.Should().Be(resultValidationObject.ErrorMessage);
                }
            }
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
        [TestCaseSource(nameof(CreatedOctopusExceptionTestData))]
        public async Task CreatedOctopusMethodNotAllowedFoundException_ShouldContainPayloadInformation(
            OctopusExceptionFactory.OctopusErrorsContract octopusErrorsContract)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);

            OctopusExceptionFactory.OctopusErrorsContract resultValidationObject = null;
            if (HasAnyData(octopusErrorsContract))
            {
                var content = JsonConvert.SerializeObject(octopusErrorsContract);
                httpResponseMessage.Content = new StringContent(content);
                resultValidationObject = OctopusExceptionFactory.OctopusErrorsContractFromBody(content);
            }

            var createdException =
                await OctopusExceptionFactory.CreateException(httpResponseMessage) as
                    OctopusMethodNotAllowedFoundException;

            using (new AssertionScope())
            {
                createdException.Should().NotBeNull();

                if (resultValidationObject != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    createdException.Message.Should().Be(resultValidationObject.ErrorMessage);
                }
            }
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
        [TestCaseSource(nameof(CreatedOctopusExceptionTestData))]
        public async Task CreatedOctopusServerException_ShouldContainPayloadInformation(
            OctopusExceptionFactory.OctopusErrorsContract octopusErrorsContract)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Ambiguous);

            OctopusExceptionFactory.OctopusErrorsContract resultValidationObject = null;
            if (HasAnyData(octopusErrorsContract))
            {
                var content = JsonConvert.SerializeObject(octopusErrorsContract);
                httpResponseMessage.Content = new StringContent(content);
                resultValidationObject = OctopusExceptionFactory.OctopusErrorsContractFromBody(content);
            }

            var createdException =
                await OctopusExceptionFactory.CreateException(httpResponseMessage) as OctopusServerException;

            using (new AssertionScope())
            {
                createdException.Should().NotBeNull();

                if (resultValidationObject != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    createdException.Message.Should().Contain(resultValidationObject.ErrorMessage);
                    createdException.HelpText.Should().Be(resultValidationObject.HelpText);

                    if (!string.IsNullOrWhiteSpace(resultValidationObject.FullException))
                    {
                        createdException.Message.Should().Contain(resultValidationObject.FullException);
                    }
                }
            }
        }

        private static bool HasAnyData(OctopusExceptionFactory.OctopusErrorsContract octopusErrorsContract)
        {
            return octopusErrorsContract.Details != null || octopusErrorsContract.Errors != null ||
                   octopusErrorsContract.ErrorMessage != null || octopusErrorsContract.FullException != null ||
                   octopusErrorsContract.HelpText != null;
        }
    }
}