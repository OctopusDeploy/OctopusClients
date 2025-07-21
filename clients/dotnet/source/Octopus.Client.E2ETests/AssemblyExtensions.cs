using System;
using System.IO;
using System.Reflection;

namespace Octopus.Client.E2ETests
{
    internal static class AssemblyExtensions
    {
        public static string FullLocalPath(this Assembly assembly)
        {
#if NETFRAMEWORK
            var codeBase = assembly.CodeBase ?? throw new NotSupportedException($"Cannot get codebase for assembly {assembly}");
#else
            var codeBase = assembly.Location;
#endif
            var uri = new UriBuilder(codeBase);
            var root = Uri.UnescapeDataString(uri.Path);
            root = root.Replace('/', Path.DirectorySeparatorChar);
            return root;
        }
    }
}