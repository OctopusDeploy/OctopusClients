using System;
using System.IO;
using System.Reflection;
using Octopus.Client.Model;

namespace Octopus.Client.E2ETests
{
    internal static class AssemblyExtensions
    {
        public static string FullLocalPath(this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var root = Uri.UnescapeDataString(uri.Path);
            root = root.Replace('/',Path.DirectorySeparatorChar);
            return root;
        }
    }
}
