using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// Factory for mapping HTTP errors into Octopus exceptions.
    /// </summary>
    public static class OctopusExceptionFactory
    {
        /// <summary>
        /// Creates the appropriate <see cref="OctopusException" /> from a HTTP response.
        /// </summary>
        /// <param name="webException">The web exception.</param>
        /// <param name="response">The response.</param>
        /// <returns>A rich exception describing the problem.</returns>
        public static OctopusException CreateException(WebException webException, HttpWebResponse response)
        {
            var statusCode = (int)response.StatusCode;

            var body = "";
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    body = reader.ReadToEnd();
                }
            }

            return CreateException(statusCode, body);
        }

        /// <summary>
        /// Creates the appropriate <see cref="OctopusException" /> from a HTTP response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A rich exception describing the problem.</returns>
        public static async Task<OctopusException> CreateException(HttpResponseMessage response)
        {
            var statusCode = (int)response.StatusCode;

            // In .NET 6, the ReadAsStringAsync extension method gracefully handles a null input.
            // In Net462, it crashes; we need this explicit check and conversion to empty-string to achieve the same behaviour.
            var body = response.Content == null ? "" : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return CreateException(statusCode, body);
        }

        public static OctopusException CreateException(int statusCode, string body)
        {
            return statusCode switch
            {
                // Bad request: usually validation error 
                400 or 409 => CreateOctopusValidationException(statusCode, body),
                // Forbidden, usually no API key or permissions
                401 or 403 => CreateOctopusSecurityException(statusCode, body),
                // Not found
                404 => new OctopusResourceNotFoundException(OctopusErrorsContractFromBody(body).ErrorMessage),
                // Method not allowed
                405 => new OctopusMethodNotAllowedFoundException(OctopusErrorsContractFromBody(body).ErrorMessage),
                _ => CreateOctopusServerException(statusCode, body)
            };
        }

        private static OctopusSecurityException CreateOctopusSecurityException(int statusCode, string body)
        {
            var errorsContract = OctopusErrorsContractFromBody(body);

            return new OctopusSecurityException(statusCode, errorsContract.ErrorMessage)
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusValidationException CreateOctopusValidationException(int statusCode, string body)
        {
            var errorsContract = OctopusErrorsContractFromBody(body);

            return new OctopusValidationException(
                statusCode,
                errorsContract.ErrorMessage,
                errorsContract.Errors,
                errorsContract.Details)
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusServerException CreateOctopusServerException(int statusCode, string body)
        {
            var errorsContract = OctopusErrorsContractFromBody(body);
            var fullDetails = errorsContract.ErrorMessage;
            if (!string.IsNullOrWhiteSpace(errorsContract.FullException))
            {
                fullDetails =
                    $"Octopus Server returned an error: {errorsContract.ErrorMessage} {Environment.NewLine}Server exception: {Environment.NewLine}{errorsContract.FullException}{Environment.NewLine} ----------------------- {Environment.NewLine}";
            }

            return new OctopusServerException(statusCode, fullDetails)
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusErrorsContract OctopusErrorsContractFromBody(string body)
        {
            OctopusErrorsContract result = null;
            string errorMessage;

            try
            {
                result = JsonConvert.DeserializeObject<OctopusErrorsContract>(body);
                errorMessage = result?.ErrorMessage ?? body;
            }
            catch (Exception ex)
            {
                errorMessage = $"Unexpected json deserialization error for {body} :: {ex.Message}";
            }

            return new OctopusErrorsContract
            {
                ErrorMessage = errorMessage,
                Errors = result?.Errors ?? Array.Empty<string>(),
                Details = result?.Details,
                HelpText = result?.HelpText ?? string.Empty,
                FullException = result?.FullException ?? string.Empty
            };
        }

        /// <summary>
        /// Error contract for error responses.
        /// </summary>
        public class OctopusErrorsContract
        {
            /// <summary>
            /// Gets or sets the error message.
            /// </summary>
            /// <value>
            /// The error message.
            /// </value>
            public string ErrorMessage { get; set; }

            /// <summary>
            /// Gets or sets the full exception.
            /// </summary>
            /// <value>
            /// The full exception if available, or null.
            /// </value>
            public string FullException { get; set; }

            /// <summary>
            /// Gets or sets the errors.
            /// </summary>
            /// <value>
            /// The errors.
            /// </value>
            public string[] Errors { get; set; }

            /// <summary>
            /// Gets or sets additional help regarding the error.
            /// </summary>
            /// <value>The help text, or null.</value>
            public string HelpText { get; set; }

            /// <summary>
            /// Gets or sets the details regarding the error
            /// </summary>
            /// <value>Structured information about the error.</value>
            public dynamic Details { get; set; }
        }
    }
}