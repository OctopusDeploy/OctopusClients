using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// Factory for mapping HTTP errors into Octopus exceptions.
    /// </summary>
    public static partial class OctopusExceptionFactory
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

            return CreateException(statusCode, body, ResponseBody.MediaTypeOf(response.ContentType), response.ResponseUri);
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

            return CreateException(
                statusCode,
                body,
                response.Content?.Headers?.ContentType?.MediaType,
                response.RequestMessage?.RequestUri);
        }

        public static OctopusException CreateException(int statusCode, string body)
            => CreateException(statusCode, body, null, null);

        private static OctopusException CreateException(int statusCode, string body, string mediaType, Uri requestUri)
        {
            var errorsContract = OctopusErrorsContractFromBody(statusCode, body, mediaType, requestUri);

            return statusCode switch
            {
                // Bad request: usually validation error 
                400 or 409 => CreateOctopusValidationException(statusCode, errorsContract),
                // Forbidden, usually no API key or permissions
                401 or 403 => CreateOctopusSecurityException(statusCode, errorsContract),
                // Not found
                404 => new OctopusResourceNotFoundException(errorsContract.ErrorMessage),
                // Method not allowed
                405 => new OctopusMethodNotAllowedFoundException(errorsContract.ErrorMessage),
                // The instance could not serve the request at all
                502 or 503 or 504 => CreateOctopusServerUnavailableException(statusCode, errorsContract),
                _ => CreateOctopusServerException(statusCode, errorsContract)
            };
        }

        private static OctopusSecurityException CreateOctopusSecurityException(int statusCode, OctopusErrorsContract errorsContract)
        {
            return new OctopusSecurityException(statusCode, errorsContract.ErrorMessage)
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusValidationException CreateOctopusValidationException(int statusCode, OctopusErrorsContract errorsContract)
        {
            return new OctopusValidationException(
                statusCode,
                errorsContract.ErrorMessage,
                errorsContract.Errors,
                errorsContract.Details)
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusServerException CreateOctopusServerException(int statusCode, OctopusErrorsContract errorsContract)
        {
            return new OctopusServerException(statusCode, errorsContract.ToServerErrorMessage())
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusServerUnavailableException CreateOctopusServerUnavailableException(int statusCode, OctopusErrorsContract errorsContract)
        {
            return new OctopusServerUnavailableException(statusCode, errorsContract.ToServerErrorMessage())
            {
                HelpText = errorsContract.HelpText
            };
        }

        private static OctopusErrorsContract OctopusErrorsContractFromBody(int statusCode, string body, string mediaType, Uri requestUri)
        {
            var result = ResponseBody.IsJson(body, mediaType) ? OctopusErrorsContract.TryDeserialize(body) : null;

            return new OctopusErrorsContract
            {
                ErrorMessage = string.IsNullOrWhiteSpace(result?.ErrorMessage)
                    ? DescribeResponse(statusCode, body, mediaType, requestUri)
                    : result.ErrorMessage,
                Errors = result?.Errors ?? Array.Empty<string>(),
                Details = result?.Details,
                HelpText = result?.HelpText ?? string.Empty,
                FullException = result?.FullException ?? string.Empty
            };
        }

        /// <summary>
        /// Describes the response itself, for when its body carries no Octopus error message: a maintenance page, a
        /// gateway error, or nothing at all.
        /// </summary>
        private static string DescribeResponse(int statusCode, string body, string mediaType, Uri requestUri)
        {
            var server = requestUri == null
                ? "Octopus Server"
                : $"Octopus Server at {requestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped)}";

            var status = Enum.IsDefined(typeof(HttpStatusCode), statusCode)
                ? $"{statusCode} {(HttpStatusCode)statusCode}"
                : statusCode.ToString();

            if (string.IsNullOrWhiteSpace(body))
                return $"{server} returned {status} with an empty response body.";

            var contentType = string.IsNullOrWhiteSpace(mediaType) ? "" : $" ({mediaType})";
            var summary = ResponseBody.Summarise(body);

            if (string.IsNullOrWhiteSpace(summary))
                return $"{server} returned {status}{contentType} with no readable content in the response body.";

            return $"{server} returned {status}{contentType}: \"{summary}\"";
        }
    }
}
