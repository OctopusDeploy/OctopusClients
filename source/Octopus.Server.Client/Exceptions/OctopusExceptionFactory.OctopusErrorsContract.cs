using System;
using Newtonsoft.Json;

namespace Octopus.Client.Exceptions
{
    public static partial class OctopusExceptionFactory
    {
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

            /// <summary>
            /// Reads a response body as the contract, or null when the body is not one.
            /// </summary>
            internal static OctopusErrorsContract TryDeserialize(string body)
            {
                try
                {
                    return JsonConvert.DeserializeObject<OctopusErrorsContract>(body);
                }
                catch (JsonException)
                {
                    return null;
                }
            }

            /// <summary>
            /// The message for a server error, carrying the server's own exception detail when it sent any.
            /// </summary>
            internal string ToServerErrorMessage()
            {
                if (string.IsNullOrWhiteSpace(FullException))
                    return ErrorMessage;

                return $"Octopus Server returned an error: {ErrorMessage} {Environment.NewLine}Server exception: {Environment.NewLine}{FullException}{Environment.NewLine} ----------------------- {Environment.NewLine}";
            }
        }
    }
}
