using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Octopus.Client.Exceptions;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Util
{
    static class ParameterHelper
    {
        public static Dictionary<string, object> CombineParameters(Dictionary<string, object> currentAdditionalQueryParameters, object userProvidedParameters)
        {
            if (currentAdditionalQueryParameters == null)
                currentAdditionalQueryParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (userProvidedParameters == null)
                userProvidedParameters = new { };
            var resultDictionary = GetParameters(userProvidedParameters);

            // Value from the current repository(currentAdditionalQueryParameters) overrides the one from user (userProvidedParameters)
            currentAdditionalQueryParameters.ToList().ForEach(x => resultDictionary[x.Key] = x.Value);
            return new Dictionary<string, object>(resultDictionary);
        }

        public static void ValidateSpaceParameters(SpaceQueryParameters curreSpaceQueryParameters, bool newIncludeGlobal,
            string[] newSpaceIds)
        {
            if (curreSpaceQueryParameters == null) return;

            if (curreSpaceQueryParameters.IncludeGlobal != newIncludeGlobal)
            {
                throw new InvalidSpacesLimitationParametersException();
            }

            var previouslyDefinedSpaceIdsSet = new HashSet<string>(curreSpaceQueryParameters.SpaceIds);
            if (!previouslyDefinedSpaceIdsSet.IsSupersetOf(newSpaceIds))
            {
                throw new InvalidSpacesLimitationParametersException();
            }
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