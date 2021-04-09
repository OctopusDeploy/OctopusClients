using System;
using System.Linq;
using System.Reflection;
using Octopus.Client.Extensions;
using Octopus.Client.Model;
using Octopus.TinyTypes;
using Octopus.TinyTypes.TypeConverters;

namespace Octopus.Client
{
    internal static class TinyTypeConverterRegistration
    {
        public static void RegisterTinyTypeConverters()
        {
            var assembliesToScan = AppDomain.CurrentDomain
                .GetAssemblies()
                .Union(new[] {typeof(ProjectResource).Assembly})
                .Distinct()
                .OrderBy(a => a.GetName().Name) // Just a nicety for visual inspection. There is no functional need for ordering.
                .ToArray();

            var tinyTypes = assembliesToScan
                .SelectMany(SafelyGetTypes)
                .Where(t => t.IsClosedTypeOfOpenGeneric(typeof(TinyType<>)))
                .ToArray();

            foreach (var tinyType in tinyTypes) TinyTypeConverters.RegisterConverterFor(tinyType);

            Type[] SafelyGetTypes(Assembly assembly)
            {
                try
                {
                    var types = assembly.DefinedTypes.Cast<Type>().ToArray();
                    return types;
                }
                catch (Exception)
                {
                    return new Type[0];
                }
            }
        }
    }
}