using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// Reads what a HTTP error response body can tell us when it is not, or might not be, an Octopus error.
    /// </summary>
    static class ResponseBody
    {
        /// <summary>
        /// The longest run of text quoted from a response body.
        /// </summary>
        const int MaximumSnippetLength = 500;

        /// <summary>
        /// A body is only inspected up to this length, which bounds the cost of reducing a large document to text.
        /// </summary>
        const int MaximumLengthToInspect = 20000;

        /// <summary>
        /// Whether to read the body as the Octopus error contract. A content type of JSON says so outright; otherwise
        /// the body's first character decides, which is what keeps a contract sent under a vague content type readable.
        /// </summary>
        public static bool IsJson(string body, string mediaType)
        {
            if (string.IsNullOrWhiteSpace(body))
                return false;

            if (mediaType != null
                && (mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase)
                    || mediaType.Equals("text/json", StringComparison.OrdinalIgnoreCase)
                    || mediaType.EndsWith("+json", StringComparison.OrdinalIgnoreCase)))
                return true;

            var first = FirstNonWhitespace(body);
            return first == '{' || first == '[';
        }

        /// <summary>
        /// Reduces a body to a single short line of readable text, so that a maintenance page reads as the sentence it
        /// contains rather than as its markup.
        /// </summary>
        public static string Summarise(string body)
        {
            var inspectable = body.Length > MaximumLengthToInspect
                ? body.Substring(0, MaximumLengthToInspect)
                : body;

            var text = FirstNonWhitespace(inspectable) == '<' ? StripMarkup(inspectable) : inspectable;
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text.Length > MaximumSnippetLength
                ? text.Substring(0, MaximumSnippetLength) + "..."
                : text;
        }

        /// <summary>
        /// The media type from a Content-Type header value, without parameters such as charset.
        /// </summary>
        public static string MediaTypeOf(string contentTypeHeader)
        {
            if (string.IsNullOrWhiteSpace(contentTypeHeader))
                return null;

            var separator = contentTypeHeader.IndexOf(';');
            return (separator < 0 ? contentTypeHeader : contentTypeHeader.Substring(0, separator)).Trim();
        }

        private static string StripMarkup(string body)
        {
            var text = Regex.Replace(body, @"<(script|style)\b[^>]*>.*?</\1\s*>", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // An element with no closing tag runs to the end of the document. A self-closing tag holds no content,
            // so the lookbehind keeps the text that follows one.
            text = Regex.Replace(text, @"<(script|style)\b[^>]*(?<!/)>.*$", " ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            text = Regex.Replace(text, @"<[^>]*>", " ");

            // Decoding turns escaped markup back into markup, so the tags come off again afterwards.
            text = WebUtility.HtmlDecode(text);

            return Regex.Replace(text, @"<[^>]*>", " ");
        }

        private static char FirstNonWhitespace(string body)
        {
            foreach (var character in body)
            {
                if (!char.IsWhiteSpace(character))
                    return character;
            }

            return '\0';
        }
    }
}
