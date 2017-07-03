#if SYNC_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using Octopus.Client.Extensions;

namespace Octopus.Client.Tests.Conventions
{
    public class RepositorySymetryConventions
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
                   where p.Name != "Client"
                   select new TestCaseData(p.PropertyType)
                       .SetName(p.PropertyType.Name + " (Sync)");
        }

        public static IEnumerable<TestCaseData> AsyncRepositories()
        {
            return from p in typeof(IOctopusAsyncRepository).GetProperties()
                   where p.Name != "Client"
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
                           let s = syncRepository.GetMethod(a.Name, parameters)
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
                           let a = asyncRepository.GetMethod(s.Name, parameters)
                           where a == null || !IsEquivalentReturnType(a.ReturnType, s.ReturnType)
                           where !(syncRepository.Name == "ITaskRepository" && s.Name == "WaitForCompletion")
                           select $"{s.Name}({parameters.Select(p => p.Name).CommaSeperate()})";

            var missing = missingQ.ToArray();
            if (missing.Any())
                Assert.Fail($"The following methods are present on the async {asyncRepository.Name} but not on the sync one, or have different return types:\r\n{missing.NewLineSeperate()}");

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
    }
}
#endif