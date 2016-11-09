using System;
using System.Collections.Generic;

namespace Octopus.Client.Extensions
{
    public static class StringExtensions
    {
        public static string NewLineSeperate(this IEnumerable<object> items) => string.Join(Environment.NewLine, items);
        public static string CommaSeperate(this IEnumerable<object> items) => string.Join(", ", items);
    }
}