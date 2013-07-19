using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
public static class UriExtensions
// ReSharper restore CheckNamespace
{
    public static Uri EnsureEndsWith(this Uri uri, string allUrisStartWith)
    {
        allUrisStartWith = (allUrisStartWith.EndsWith("/") ? allUrisStartWith : allUrisStartWith + "/");
        var rootUri = uri.ToString();
        rootUri = (rootUri.EndsWith("/") ? rootUri : rootUri + "/");

        var indexOfMandatorySegment = rootUri.LastIndexOf(allUrisStartWith, StringComparison.OrdinalIgnoreCase);
        if (indexOfMandatorySegment >= 1)
        {
            rootUri = rootUri.Substring(0, indexOfMandatorySegment);
        }

        var root = rootUri.TrimEnd('/');
        Console.WriteLine(root);
        return new Uri(root);
    }
}