using System;

// ReSharper disable CheckNamespace
namespace Octopus.Cli.Util
{
    public static class UriExtensions
// ReSharper restore CheckNamespace
    {
        public static Uri EnsureEndsWith(this Uri uri, string suffix)
        {
            var path = uri.AbsolutePath.ToLowerInvariant();
            suffix = suffix.ToLowerInvariant();
            var overlap = FindOverlapSection(path, suffix);
            if (!String.IsNullOrEmpty(overlap))
            {
                path = path.Replace(overlap, string.Empty);
                suffix = suffix.Replace(overlap, string.Empty);
            }
            path = path + overlap + suffix;
            path = path.Replace("//", "/");

            return new Uri(uri, path);
        }

        static string FindOverlapSection(string value1, string value2)
        {
            var longer = value1;
            var shorter = value2;
            if (shorter.Length > longer.Length)
            {
                var temp = longer;
                longer = shorter;
                shorter = temp;
            }

            return longer.Contains(shorter) ? shorter : String.Empty;
        }
    }
}