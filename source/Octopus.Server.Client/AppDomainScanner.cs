using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Octopus.Client
{
    internal static class AppDomainScanner
    {
        public static Type[] ScanForAllTypes()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.DefinedTypes.ToArray();
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        return new Type[0];
                    }
                    catch (TypeLoadException)
                    {
                        return new Type[0];
                    }
                })
                .Where(t => !typeof(IAsyncStateMachine).IsAssignableFrom(t))
                .OrderBy(t => t.FullName)
                .ToArray();

            return types;
        }
    }
}