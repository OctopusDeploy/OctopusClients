using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Octopus.Client.Util
{
    public static class ParameterHelper
    {
        public static Dictionary<string, object> CombineParameters(object leftHandSide, object rightHandSide)
        {
            if (leftHandSide == null)
                leftHandSide = new { };

            if (rightHandSide == null)
                rightHandSide = new { };
            return GetParameters(leftHandSide).Union(GetParameters(rightHandSide)).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        static Dictionary<string, object> GetParameters(object parameters)
        {
            var dictionary =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var properties = parameters.GetType().GetTypeInfo().GetProperties();
            foreach (var property in properties)
            {
                dictionary[property.Name] = property.GetValue(parameters, null);
            }

            return dictionary;
        }
    }

}