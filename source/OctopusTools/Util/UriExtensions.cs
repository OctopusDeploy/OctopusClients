using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
public static class UriExtensions
// ReSharper restore CheckNamespace
{
    public static Uri EnsureEndsWith(this Uri uri, string suffix)
    {
        var path = uri.AbsolutePath;
        if (!path.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
        {
            path = path + suffix;
        }

        path = path.Replace("//", "/");

        return new Uri(uri, path);
    }
}