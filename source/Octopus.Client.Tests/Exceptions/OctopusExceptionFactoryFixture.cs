using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;
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
        [TestCaseSource(nameof(CreatedOctopusValidationExceptionTestData))]
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

        public static IEnumerable<TestCaseData> CreatedOctopusValidationExceptionTestData()
        {
            const int httpStatusCode = (int)HttpStatusCode.BadRequest;
            const string errorMessageValue = "Error Message";
            const string helpTextValue = "Help Text";
            var errorsValue = new[] { "Additional Errors" };
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "Errors", JArray.FromObject(errorsValue) },
                { "HelpText", helpTextValue },
                { "Random", "not relevant"}
            };

            yield return new TestCaseData(jObject.ToString(), new OctopusValidationException(httpStatusCode, errorMessageValue, errorsValue) { HelpText = helpTextValue });

            jObject.Remove("Errors");
            yield return new TestCaseData(jObject.ToString(), new OctopusValidationException(httpStatusCode, errorMessageValue, Array.Empty<string>()) { HelpText = helpTextValue });

            jObject.Remove("HelpText");
            yield return new TestCaseData(jObject.ToString(), new OctopusValidationException(httpStatusCode, errorMessageValue, Array.Empty<string>()) { HelpText = string.Empty });
            
            yield return new TestCaseData(string.Empty, new OctopusValidationException(httpStatusCode, string.Empty, Array.Empty<string>()) { HelpText = string.Empty });
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
        [TestCaseSource(nameof(CreatedOctopusSecurityExceptionTestData))]
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
        
        public static IEnumerable<TestCaseData> CreatedOctopusSecurityExceptionTestData()
        {
            const int httpStatusCode = (int)HttpStatusCode.Unauthorized;
            const string errorMessageValue = "Error Message";
            const string helpTextValue = "Help Text";
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "HelpText", helpTextValue },
                { "Random", "not relevant"}
            };

            yield return new TestCaseData(jObject.ToString(), new OctopusSecurityException(httpStatusCode, errorMessageValue) { HelpText = helpTextValue });

            jObject.Remove("HelpText");
            yield return new TestCaseData(jObject.ToString(), new OctopusSecurityException(httpStatusCode, errorMessageValue) { HelpText = string.Empty});
            
            yield return new TestCaseData(string.Empty, new OctopusSecurityException(httpStatusCode, string.Empty) { HelpText = string.Empty});
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
        [TestCaseSource(nameof(CreatedOctopusResourceNotFoundExceptionTestData))]
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
        
        public static IEnumerable<TestCaseData> CreatedOctopusResourceNotFoundExceptionTestData()
        {
            const string errorMessageValue = "Error Message";
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "Random", "not relevant"}
            };

            yield return new TestCaseData(jObject.ToString(), new OctopusResourceNotFoundException(errorMessageValue));
            yield return new TestCaseData(string.Empty, new OctopusResourceNotFoundException(string.Empty));
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
        [TestCaseSource(nameof(CreatedOctopusMethodNotAllowedFoundExceptionTestData))]
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

        public static IEnumerable<TestCaseData> CreatedOctopusMethodNotAllowedFoundExceptionTestData()
        {
            const string errorMessageValue = "Error Message";
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "Random", "not relevant"}
            };

            yield return new TestCaseData(jObject.ToString(), new OctopusMethodNotAllowedFoundException(errorMessageValue));
            yield return new TestCaseData(string.Empty, new OctopusMethodNotAllowedFoundException(string.Empty));
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
        [TestCaseSource(nameof(CreatedOctopusServerExceptionTestData))]
        public async Task CreatedOctopusServerException_ShouldContainPayloadInformation(string payload, OctopusServerException expectedResult)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Ambiguous);

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
        
        public static IEnumerable<TestCaseData> CreatedOctopusServerExceptionTestData()
        {
            const int httpStatusCode = (int)HttpStatusCode.Ambiguous;
            const string errorMessageValue = "Error Message";
            const string helpTextValue = "Help Text";
            const string fullExceptionValue = "Full Exception";
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "HelpText", helpTextValue },
                { "FullException", fullExceptionValue },
                { "Random", "not relevant"}
            };

            yield return new TestCaseData(jObject.ToString(), new OctopusServerException(httpStatusCode, fullExceptionValue) { HelpText = helpTextValue });

            jObject.Remove("FullException");
            yield return new TestCaseData(jObject.ToString(), new OctopusServerException(httpStatusCode, errorMessageValue) { HelpText = helpTextValue });
            
            jObject.Remove("HelpText");
            yield return new TestCaseData(jObject.ToString(), new OctopusServerException(httpStatusCode, errorMessageValue) { HelpText = string.Empty });
            
            yield return new TestCaseData(string.Empty, new OctopusServerException(httpStatusCode, string.Empty) { HelpText = string.Empty });
        }
    }
}