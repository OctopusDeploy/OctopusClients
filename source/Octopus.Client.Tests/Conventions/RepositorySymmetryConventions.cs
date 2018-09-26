#if SYNC_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Octopus.Client.Extensions;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Conventions
{
    public class RepositorySymmetryConventions
    {
        [Test]
        public void IOctopusAsyncRepositoryExposesTheSamePropertiesAsIOctopusRepository()
        {
            var syncProps = typeof(IOctopusRepository).GetProperties().ToArray();
            var different = typeof(IOctopusAsyncRepository).GetProperties()
                .Where(a => a.Name != "Client")
                .Where(a => !syncProps.Any(s => s.Name == a.Name && s.PropertyType.Name == a.PropertyType.Name))
                .Select(p => p.Name)
                .ToArray();

            if (different.Any())
                Assert.Fail($"The following properties are present on IOctopusRepository but not on IOctopusAsyncRepository, or have different type names: " + different.CommaSeperate());
        }

        [Test]
        public void IOctopusRepositoryExposesTheSamePropertiesAsIOctopusAsyncRepository()
        {
            var syncProps = typeof(IOctopusAsyncRepository).GetProperties().ToArray();
            var different = typeof(IOctopusRepository).GetProperties()
                .Where(a => a.Name != "Client")
                .Where(a => !syncProps.Any(s => s.Name == a.Name && s.PropertyType.Name == a.PropertyType.Name))
                .Select(p => p.Name)
                .ToArray();

            if (different.Any())
                Assert.Fail($"The following properties are present on IOctopusRepository but not on IOctopusAsyncRepository, or have different type names: " + different.CommaSeperate());
        }

        public static IEnumerable<TestCaseData> SyncRepositories()
        {
            return from p in typeof(IOctopusRepository).GetProperties()
                   where p.Name != "Client" && p.Name != nameof(SpaceContext) && p.Name != "SpaceRootDocument" && p.Name != "RootDocument"
                   select new TestCaseData(p.PropertyType)
                       .SetName(p.PropertyType.Name + " (Sync)");
        }

        public static IEnumerable<TestCaseData> AsyncRepositories()
        {
            return from p in typeof(IOctopusAsyncRepository).GetProperties()
                   where p.Name != "Client" && p.Name != nameof(SpaceContext) && p.Name != "SpaceRootDocument" && p.Name != "RootDocument"
                   select new TestCaseData(p.PropertyType)
                       .SetName(p.PropertyType.Name + " (Async)");
        }

        [TestCaseSource(nameof(AsyncRepositories))]
        public void AllAsyncRepositoriesShouldHaveEquivalentSignaturesToTheSyncRepositories(Type asyncRepository)
        {
            var syncRepository = typeof(IOctopusAsyncRepository).Assembly
                .GetExportedTypes()
                .FirstOrDefault(t => t.Name == asyncRepository.Name && !t.Namespace.EndsWith("Async"));

            if (syncRepository == null)
                Assert.Fail("Sync repository not found");

            var missingQ = from a in asyncRepository.GetMethods()
                           let parameters = a.GetParameters().Select(p => p.ParameterType).ToArray()
                           let s = GetMatchingMethod(a, syncRepository, parameters)
                           where s == null || !IsEquivalentReturnType(a.ReturnType, s.ReturnType)
                           where !(asyncRepository.Name == "ITaskRepository" && a.Name == "WaitForCompletion")
                           select $"{a.Name}({parameters.Select(p => p.Name).CommaSeperate()})";

            var missing = missingQ.ToArray();

            if (missing.Any())
                Assert.Fail($"The following methods are present on the sync {syncRepository.Name} but not on the async one, or have different return types:\r\n{missing.NewLineSeperate()}");

        }

        [TestCaseSource(nameof(SyncRepositories))]
        public void AllSyncRepositoriesShouldHaveEquivalentSignaturesToTheAsyncRepositories(Type syncRepository)
        {
            var asyncRepository = typeof(IOctopusAsyncRepository).Assembly
                .GetExportedTypes()
                .FirstOrDefault(t => t.Name == syncRepository.Name && t.Namespace.EndsWith("Async"));

            if (asyncRepository == null)
                Assert.Fail("Async repository not found");

            var missingQ = from s in syncRepository.GetMethods()
                           let parameters = s.GetParameters().Select(p => p.ParameterType).ToArray()
                           let a = GetMatchingMethod(s, asyncRepository, parameters)
                           where a == null || !IsEquivalentReturnType(a.ReturnType, s.ReturnType)
                           where !(syncRepository.Name == "ITaskRepository" && s.Name == "WaitForCompletion")
                           select $"{s.Name}({parameters.Select(p => p.Name).CommaSeperate()})";

            var missing = missingQ.ToArray();
            if (missing.Any())
                Assert.Fail($"The following methods are present on the async {asyncRepository.Name} but not on the sync one, or have different return types:\r\n{missing.NewLineSeperate()}");

        }

        [TestCaseSource(nameof(AsyncRepositories))]
        public void AllAsyncRepositoriesShouldImplementEquivalentInterfacesToTheSyncRepositories(Type asyncRepository)
        {
            var syncRepository = typeof(IOctopusAsyncRepository).Assembly
                .GetExportedTypes()
                .FirstOrDefault(t => t.Name == asyncRepository.Name && !t.Namespace.EndsWith("Async"));

            if (syncRepository == null)
                Assert.Fail("Sync repository not found");

            var asyncInterfaces = asyncRepository.GetInterfaces().Select(i => i.ToString().Replace(i.Namespace, string.Empty).TrimStart('.')).ToArray();
            var syncInterfaces = syncRepository.GetInterfaces().Select(i => i.ToString().Replace(i.Namespace, string.Empty).TrimStart('.')).ToArray();

            var missing = syncInterfaces.Except(asyncInterfaces).ToArray();

            if (missing.Any())
                Assert.Fail($"The following interfaces are implemented on the sync {syncRepository.Name} but not on the async one:\r\n{missing.NewLineSeperate()}");
        }

        [TestCaseSource(nameof(SyncRepositories))]
        public void AllSyncRepositoriesShouldImplementEquivalentInterfacesToTheAsyncRepositories(Type syncRepository)
        {
            var asyncRepository = typeof(IOctopusAsyncRepository).Assembly
                .GetExportedTypes()
                .FirstOrDefault(t => t.Name == syncRepository.Name && t.Namespace.EndsWith("Async"));

            if (asyncRepository == null)
                Assert.Fail("Async repository not found");

            var asyncInterfaces = asyncRepository.GetInterfaces().Select(i => i.ToString().Replace(i.Namespace, string.Empty).TrimStart('.')).ToArray();
            var syncInterfaces = syncRepository.GetInterfaces().Select(i => i.ToString().Replace(i.Namespace, string.Empty).TrimStart('.')).ToArray();

            var missing = asyncInterfaces.Except(syncInterfaces).ToArray();

            if (missing.Any())
                Assert.Fail($"The following interfaces are implemented on the async {asyncRepository.Name} but not on the sync one:\r\n{missing.NewLineSeperate()}");

        }

        bool IsEquivalentReturnType(Type asyncType, Type syncType)
        {
            if (asyncType.Name == syncType.Name)
                return true;
            if (asyncType == typeof(Task) && syncType == typeof(void))
                return true;
            if (!asyncType.IsGenericType)
                return false;
            var generic = asyncType.GetGenericTypeDefinition();
            if (generic != typeof(Task<>))
                return false;
            return syncType.Name == asyncType.GetGenericArguments()[0].Name;
        }

        private MethodInfo GetMatchingMethod(MethodInfo sourceMethod, Type targetType, Type[] parameters)
        {
            return sourceMethod.IsGenericMethodDefinition
                ? targetType.GetMethods().First(x => x.Name == sourceMethod.Name && x.GetParameters().SequenceEqual(sourceMethod.GetParameters(), new SimpleParameterComparer()))
                : targetType.GetMethod(sourceMethod.Name, parameters);
        }

        private class SimpleParameterComparer : IEqualityComparer<ParameterInfo>
        {
            public bool Equals(ParameterInfo x, ParameterInfo y)
            {
                var parameterTypeEqual = x.ParameterType.IsGenericParameter
                    ? x.ParameterType.Name == y.ParameterType.Name && x.ParameterType.IsGenericParameter == y.ParameterType.IsGenericParameter
                    : x.ParameterType.FullName == y.ParameterType.FullName && x.ParameterType.AssemblyQualifiedName == y.ParameterType.AssemblyQualifiedName;

                return x.Position == y.Position && parameterTypeEqual;
            }

            public int GetHashCode(ParameterInfo obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
#endif