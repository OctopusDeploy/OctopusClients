using System;
using System.Collections.Generic;

namespace Octopus.Cli.Util
{
    public static class StringExtensions
    {
        public static string CommaSeperate(this IEnumerable<object> items)
        {
            return string.Join(", ", items);
        }

        public static string NewlineSeperate(this IEnumerable<object> items)
        {
            return string.Join(Environment.NewLine, items);
        }
    }
}