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
#if SYNC_CLIENT
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
#endif

        /// <summary>
        /// Creates the appropriate <see cref="OctopusException" /> from a HTTP response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A rich exception describing the problem.</returns>
        public static async Task<OctopusException> CreateException(HttpResponseMessage response)
        {
            var statusCode = (int)response.StatusCode;

            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return CreateException(statusCode, body);
        }

        static OctopusException CreateException(int statusCode, string body)
        {
            if (statusCode == 400 || statusCode == 409) // Bad request: usually validation error 
            {
                var errors = JsonConvert.DeserializeObject<OctopusErrorsContract>(body);
                return new OctopusValidationException(statusCode, errors.ErrorMessage, errors.Errors, errors.Details)
                {
                    HelpText = errors.HelpText
                };
            }

            if (statusCode == 401 || statusCode == 403) // Forbidden, usually no API key or permissions
            {
                var errors = JsonConvert.DeserializeObject<OctopusErrorsContract>(body);
                errors = errors ?? new OctopusErrorsContract
                         {
                             ErrorMessage =
                                 $"The server returned a status code of {statusCode}: {body}"
                         };
                return new OctopusSecurityException(statusCode, errors.ErrorMessage) {HelpText = errors.HelpText};
            }

            if (statusCode == 404) // Not found
            {
                var errorMessage = GetErrorMessage(body);
                return new OctopusResourceNotFoundException(errorMessage);
            }

            if (statusCode == 405) // Method not allowed
            {
                var errorMessage = GetErrorMessage(body);
                return new OctopusMethodNotAllowedFoundException(errorMessage);
            }

            var fullDetails = body;
            string helpText = null;
            try
            {
                var errors = JsonConvert.DeserializeObject<OctopusErrorsContract>(body);
                if (errors != null)
                {
                    fullDetails = "Octopus Server returned an error: " + errors.ErrorMessage;
                    helpText = errors.HelpText;
                    if (!string.IsNullOrWhiteSpace(errors.FullException))
                    {
                        fullDetails += Environment.NewLine + "Server exception: " + Environment.NewLine + errors.FullException + Environment.NewLine + "-----------------------" + Environment.NewLine;
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return new OctopusServerException(statusCode, fullDetails) {HelpText = helpText};
        }

        static string GetErrorMessage(string body)
        {
            string errorMessage;
            try
            {
                var errors = JsonConvert.DeserializeObject<OctopusErrorsContract>(body);
                errorMessage = errors.ErrorMessage;
            }
            catch
            {
                errorMessage = body;
            }
            return errorMessage;
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