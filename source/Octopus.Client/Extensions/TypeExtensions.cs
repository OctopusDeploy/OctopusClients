using System;
using System.Reflection;

namespace Octopus.Client.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type t) => t.GetTypeInfo().IsValueType ? Activator.CreateInstance(t) : null;
    }
}