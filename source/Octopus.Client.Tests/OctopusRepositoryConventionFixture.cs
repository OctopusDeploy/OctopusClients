using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Octopus.Client.Tests
{
    [TestFixture]
    public class OctopusRepositoryConventionFixture
    {
        [Test]
        public void EnsureAllRepositoryPropertiesHaveBeenAdded()
        {
            var rootRepo = typeof(IOctopusAsyncRepository);
            var spaceRepo = typeof(ISpaceScopedAsyncRepository);
            var systemRepo = typeof(ISystemScopedAsyncRepository);
            var mixedScopeRepo = typeof(IMixedScopeAsyncRepository);
            var interfaceProps = rootRepo.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository"))
                .Concat(spaceRepo.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository")))
                .Concat(systemRepo.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository")))
                .Concat(mixedScopeRepo.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository")))
                .ToList();
            var t = typeof(OctopusAsyncRepository);
            var implementationProps = t.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository")).ToList();

            Assert.That(interfaceProps.Count(), Is.EqualTo(implementationProps.Count()), GetDifferenceMessage(interfaceProps, implementationProps));
        }

        static string GetDifferenceMessage(IEnumerable<PropertyInfo> interfaceProps, IEnumerable<PropertyInfo> implementationProps)
        {

            var ints = interfaceProps.Select(p => $"({p.PropertyType.Name}){p.Name}");
            var imps = implementationProps.Select(p => $"({p.PropertyType.Name}){p.Name}");
            var msg = "IOctopusAsyncRepository and OctopusRepository repository properties are out-of-sync";
            var missingInInts = imps.Except(ints);

            if (missingInInts.Any())
            {
                msg += "\nMissing on Interface: " + string.Join(", ", missingInInts);
            }

            return msg;
        }
    }
}
