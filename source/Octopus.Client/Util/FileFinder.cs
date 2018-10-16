using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Octopus.Client.Logging.LogProviders;

namespace Octopus.Client.Util
{
    internal static class FileFinder
    {
        public static string ResolveFileRelativeToType(string filename, Type type)
        {
            var bestMatch = Path.GetFullPath(filename);
            if (File.Exists(bestMatch))
                return bestMatch;

            var assembly = type.GetAssemblyPortable();
            var root = GetAssemblyLocation(assembly);
            bestMatch = Path.Combine(root, filename);
            if (File.Exists(bestMatch))
                return bestMatch;

            var namespaceSegments = type.Namespace?.Split('.');
            if (namespaceSegments == null)
                return null;

            var namespaceStack = new Stack<string>(namespaceSegments);
            while (namespaceStack.Count > 0)
            {
                filename = Path.Combine(namespaceStack.Pop(), filename);
                var full = Path.GetFullPath(filename);
                if (File.Exists(full))
                    return full;
            }
            return null;
        }

        static string GetAssemblyLocation(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
