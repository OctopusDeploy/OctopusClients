using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Octopus.Client.Util
{
    public static class ParameterHelper
    {
        public static Dictionary<string, object> CombineParameters(Dictionary<string, object> spaceLimitedParameters, object userProvidedParameters)
        {
            if (spaceLimitedParameters == null)
                spaceLimitedParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (userProvidedParameters == null)
                userProvidedParameters = new { };
            var resultDictionary = GetParameters(userProvidedParameters);

            // Value from the current repository(spaceLimitedParameters) overrides the one from user (userProvidedParameters)
            spaceLimitedParameters.ToList().ForEach(x => resultDictionary[x.Key] = x.Value);
            return new Dictionary<string, object>(resultDictionary);
        }

        static IDictionary<string, object> GetParameters(object parameters)
        {
            if (parameters is IDictionary<string, object> maybeDictionary)
                return maybeDictionary;
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            var properties = parameters.GetType().GetTypeInfo().GetProperties();
            foreach (var property in properties)
            {
                dictionary[property.Name] = property.GetValue(parameters, null);
            }

            return dictionary;
        }
    }

}