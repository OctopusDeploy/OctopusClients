using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Tests.Extensions
{
    internal static class TypeExtensions
    {
        //source: https://stackoverflow.com/a/1823664
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            if (type.BaseType == null) return type.GetInterfaces();

            return Enumerable.Repeat(type.BaseType, 1)
                             .Concat(type.GetInterfaces())
                             .Concat(type.GetInterfaces().SelectMany(GetBaseTypes))
                             .Concat(type.BaseType.GetBaseTypes())
                             .Distinct();
        }
    }
}
