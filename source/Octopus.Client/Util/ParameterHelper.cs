using System;
using System.Collections.Generic;
using System.Reflection;

namespace Octopus.Client.Util
{
    public static class ParameterHelper
    {
        public static Dictionary<string, object> CombineParameters(object parameters, bool includeGlobal, string[] spaceIds)
        {
            if (parameters == null)
                parameters = new { };

            var dictionary =
                new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeGlobal"] = includeGlobal,
                    ["spaces"] = spaceIds
                };

            var properties = parameters.GetType().GetTypeInfo().GetProperties();
            foreach (var property in properties)
            {
                dictionary[property.Name] = property.GetValue(parameters, null);
            }

            return dictionary;
        }
    }

}