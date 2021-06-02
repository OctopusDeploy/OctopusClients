using System;
using System.IO;
using System.Reflection;
using Octopus.Client.Model;

namespace Octopus.Client.Extensions
{
    internal static class AssemblyExtensions
    {
        public static string GetInformationalVersion(this Type type)
        {
            return type.GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public static SemanticVersion GetSemanticVersion(this Type type)
        {
            return new SemanticVersion(type.GetInformationalVersion());
        }
    }
}