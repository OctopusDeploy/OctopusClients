using System;
using System.Linq;
using System.Collections.Generic;

namespace Octopus.Client.Tests.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveNewlines(this string str)
        {
            return str?.Replace("\r", "").Replace("\n", "");
        }

        public static string NewLineSeperate(this IEnumerable<object> items) => string.Join(Environment.NewLine, items);
        public static string CommaSeperate(this IEnumerable<object> items) => string.Join(", ", items);

        public static string[] InArray(this string str) => new[] {str};
        public static IEnumerable<string> Concat(this IEnumerable<string> items, string str) => items.Concat(new[] { str });
    }
}