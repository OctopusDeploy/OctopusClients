using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Octopus.Client.Exceptions;

namespace Octopus.Client.Tests.Exceptions
{
    /// <summary>
    /// Error response bodies paired with the exception each should produce, for the payload cases in
    /// <see cref="OctopusExceptionFactoryFixture" />.
    /// </summary>
    static class ErrorPayloads
    {
        public static IEnumerable<TestCaseData> ForValidationExceptions()
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

            //400 (BadRequest) error returned
            yield return new TestCaseData(jObject.ToString(), new OctopusValidationException(httpStatusCode, errorMessageValue, errorsValue) { HelpText = helpTextValue });

            //400 (BadRequest) error structure without any `Errors`
            jObject.Remove("Errors");
            yield return new TestCaseData(jObject.ToString(), new OctopusValidationException(httpStatusCode, errorMessageValue, Array.Empty<string>()) { HelpText = helpTextValue });

            //400 (BadRequest) error structure without any `HelpText` nor `Errors`
            jObject.Remove("HelpText");
            yield return new TestCaseData(jObject.ToString(), new OctopusValidationException(httpStatusCode, errorMessageValue, Array.Empty<string>()) { HelpText = string.Empty });

            //400 (BadRequest) error without a body 
            yield return new TestCaseData(string.Empty, new OctopusValidationException(httpStatusCode, ErrorResponses.EmptyBodyMessage(httpStatusCode), Array.Empty<string>()) { HelpText = string.Empty });
        }

        public static IEnumerable<TestCaseData> ForSecurityExceptions()
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

            //401 (Unauthorized) error returned
            yield return new TestCaseData(jObject.ToString(), new OctopusSecurityException(httpStatusCode, errorMessageValue) { HelpText = helpTextValue });

            //401 (Unauthorized) error structure without any `HelpText`
            jObject.Remove("HelpText");
            yield return new TestCaseData(jObject.ToString(), new OctopusSecurityException(httpStatusCode, errorMessageValue) { HelpText = string.Empty });

            //401 (Unauthorized) error returned a body 
            yield return new TestCaseData(string.Empty, new OctopusSecurityException(httpStatusCode, ErrorResponses.EmptyBodyMessage(httpStatusCode)) { HelpText = string.Empty });
        }

        public static IEnumerable<TestCaseData> ForResourceNotFoundExceptions()
        {
            const string errorMessageValue = "Error Message";
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "Random", "not relevant"}
            };

            //404 (Not Found) error structure returned
            yield return new TestCaseData(jObject.ToString(), new OctopusResourceNotFoundException(errorMessageValue));
            //404 (Not Found) error returned without a body 
            yield return new TestCaseData(string.Empty, new OctopusResourceNotFoundException(ErrorResponses.EmptyBodyMessage((int)HttpStatusCode.NotFound)));
        }

        public static IEnumerable<TestCaseData> ForMethodNotAllowedExceptions()
        {
            const string errorMessageValue = "Error Message";
            var jObject = new JObject
            {
                { "ErrorMessage", errorMessageValue },
                { "Random", "not relevant"}
            };

            //Structured error returned
            yield return new TestCaseData(jObject.ToString(), new OctopusMethodNotAllowedFoundException(errorMessageValue));
            //405 (Not Allowed) returned without a body 
            yield return new TestCaseData(string.Empty, new OctopusMethodNotAllowedFoundException(ErrorResponses.EmptyBodyMessage((int)HttpStatusCode.MethodNotAllowed)));
        }

        public static IEnumerable<TestCaseData> ForServerExceptions()
        {
            const int httpStatusCode = (int)HttpStatusCode.InternalServerError;
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

            //500 (InternalServerError) with a full error + exception 
            yield return new TestCaseData(jObject.ToString(), new OctopusServerException(httpStatusCode, fullExceptionValue) { HelpText = helpTextValue });

            //500 (InternalServerError) with a full error, but no exception
            jObject.Remove("FullException");
            yield return new TestCaseData(jObject.ToString(), new OctopusServerException(httpStatusCode, errorMessageValue) { HelpText = helpTextValue });

            //500 (InternalServerError) with a full error but no exception or HelpText
            jObject.Remove("HelpText");
            yield return new TestCaseData(jObject.ToString(), new OctopusServerException(httpStatusCode, errorMessageValue) { HelpText = string.Empty });

            //500 (InternalServerError) with an empty body returned
            yield return new TestCaseData(string.Empty, new OctopusServerException(httpStatusCode, ErrorResponses.EmptyBodyMessage(httpStatusCode)) { HelpText = string.Empty });
        }
    }
}
