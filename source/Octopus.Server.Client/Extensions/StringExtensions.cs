using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Extensions
{
    public static class StringExtensions
    {
        public static string StringJoin(this IEnumerable<string> items, string separator)
            => string.Join(separator, items);

        public static string NewLineSeparate(this IEnumerable<object> items)
            => items.Select(item => item.ToString())
                .StringJoin(Environment.NewLine);

        public static string CommaSeparate(this IEnumerable<object> items)
            => items.Select(item => item.ToString())
                .StringJoin(", ");

        public static string ToCamelCase(this string s)
        {
            if (s.Length <= 1) return s.ToLowerInvariant();

            var first = s.Substring(0, 1).ToLowerInvariant();
            var remainder = s.Substring(1);
            var result = first + remainder;
            return result;
        }
    }
}