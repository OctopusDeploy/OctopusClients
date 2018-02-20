using System;
using System.Collections.Generic;
using Octopus.Cli.Infrastructure;

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

        public static void CheckForIllegalPathCharacters(this string path, string name)
        {
            if (path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
            {
                throw new CommandException($"Argument {name} has a value of {path} which contains invalid path characters. If your path has a trailing backslash either remove it or escape it correctly by using \\\\");
            }
        }
    }
}