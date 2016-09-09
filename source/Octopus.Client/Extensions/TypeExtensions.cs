using System;

namespace Octopus.Client.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
    }
}