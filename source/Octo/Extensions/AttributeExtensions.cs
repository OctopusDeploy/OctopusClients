using System;
using System.Linq;
using System.Reflection;

namespace Octopus.Cli.Extensions
{
    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var att = type.GetTypeInfo().GetCustomAttributes(typeof (TAttribute), true)
                .FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
    }
}