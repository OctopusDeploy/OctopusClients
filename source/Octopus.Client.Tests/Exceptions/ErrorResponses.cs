using System.Net;
using System.Net.Http;
using System.Text;

namespace Octopus.Client.Tests.Exceptions
{
    /// <summary>
    /// The HTTP error responses <see cref="OctopusExceptionFactoryFixture" /> reads, and the message it expects from
    /// one carrying no body.
    /// </summary>
    static class ErrorResponses
    {
        /// <summary>
        /// A cloud instance's maintenance page. The reason the instance is unavailable sits in the markup alongside a
        /// title, a stylesheet and a reload script, none of which belong in an exception message.
        /// Modelled on, not copied from, the page nginx serves. That one is defined in HostedScripts under the
        /// configure_kubernetes_add_octopus_server_is_undergoing_maintenance_page step:
        /// https://github.com/OctopusDeploy/HostedScripts/blob/main/source/Hub/configure.kubernetes.tf
        /// </summary>
        public const string MaintenancePage = @"<!DOCTYPE html>
<html><head><meta charset=""utf-8""><title>Maintenance</title>
<style>body { font-family: sans-serif; }</style>
<script>window.setTimeout(function () { location.reload(); }, 30000);</script>
</head>
<body><h1>Down for maintenance</h1>
<p>This instance is currently undergoing maintenance &amp; will be back soon.</p>
</body></html>";

        /// <summary>
        /// A maintenance page whose script tag is self-closing. Nothing closes the element, so a stripper that treats
        /// every script tag as an opener runs to the end of the document and discards the reason for the outage.
        /// </summary>
        public const string MaintenancePageWithSelfClosingScript = @"<!DOCTYPE html>
<html><head><title>Maintenance</title><script src=""/reload.js"" /></head>
<body><h1>Down for maintenance</h1>
<p>This instance is currently undergoing maintenance and will be back soon.</p>
</body></html>";

        /// <summary>
        /// A body whose only content is a stylesheet, leaving nothing readable behind once markup is stripped.
        /// </summary>
        public const string PageWithNoReadableText = @"<!DOCTYPE html>
<html><head><style>body { color: #333; }</style></head><body></body></html>";

        /// <summary>
        /// A body that escapes markup so it displays as text rather than running.
        /// </summary>
        public const string PageWithEscapedMarkup =
            @"<html><body><p>&lt;script&gt;alert(1)&lt;/script&gt; undergoing maintenance</p></body></html>";

        /// <summary>
        /// What the maintenance page becomes for a JSON caller once the ingress negotiates on the Accept header, added
        /// by https://github.com/OctopusDeploy/HostedScripts/pull/457. Until that ships, a cloud instance answers with
        /// the HTML page whatever the caller asked for. ErrorSubCode and Category are absent from the client's error
        /// contract and are ignored.
        /// </summary>
        public const string MaintenanceJson =
            @"{""ErrorSubCode"": ""InstanceUndergoingMaintenance"", ""Category"": ""InstanceUnavailable"", ""ErrorMessage"": ""Your Octopus Cloud instance is currently undergoing maintenance and will be back soon."", ""Errors"": [""Your Octopus Cloud instance is currently undergoing maintenance and will be back soon.""]}";

        public static HttpResponseMessage With(HttpStatusCode statusCode, string body = null, string mediaType = null, string requestUri = null)
        {
            var response = new HttpResponseMessage(statusCode);

            if (body != null)
            {
                response.Content = new StringContent(body, Encoding.UTF8, mediaType);

                // StringContent falls back to text/plain for a null media type, which is a different case
                // from a response that carries no Content-Type at all.
                if (mediaType == null)
                    response.Content.Headers.ContentType = null;
            }

            if (requestUri != null)
                response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

            return response;
        }

        /// <summary>
        /// The message expected of a response that carried no body, at any status code.
        /// </summary>
        public static string EmptyBodyMessage(int statusCode)
            => $"Octopus Server returned {statusCode} {(HttpStatusCode)statusCode} with an empty response body.";
    }
}
