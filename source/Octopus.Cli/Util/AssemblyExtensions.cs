using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// ReSharper disable CheckNamespace
public static class AssemblyExtensions
// ReSharper restore CheckNamespace
{
    public static string FullLocalPath(this Assembly assembly)
    {
        var codeBase = assembly.CodeBase;
        var uri = new UriBuilder(codeBase);
        var root = Uri.UnescapeDataString(uri.Path);
        root = root.Replace("/", "\\");
        return root;
    }

    public static string GetInformationalVersion(this Assembly assembly)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        return fileVersionInfo.ProductVersion;
    }
}