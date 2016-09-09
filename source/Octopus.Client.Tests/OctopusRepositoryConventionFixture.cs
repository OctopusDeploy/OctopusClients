using System;
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
            var i = typeof(IOctopusRepository);
            var interfaceProps = i.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository")).ToList();
            var t = typeof(OctopusRepository);
            var implementationProps = t.GetProperties().Where(p => p.PropertyType.Name.EndsWith("Repository")).ToList();

            Assert.That(interfaceProps.Count(), Is.EqualTo(implementationProps.Count()), GetDifferenceMessage(interfaceProps, implementationProps));
        }

        static string GetDifferenceMessage(IEnumerable<PropertyInfo> interfaceProps, IEnumerable<PropertyInfo> implementationProps)
        {

            var ints = interfaceProps.Select(p => $"({p.PropertyType.Name}){p.Name}");
            var imps = implementationProps.Select(p => $"({p.PropertyType.Name}){p.Name}");
            var msg = "IOctopusRepository and OctopusRepository repository properties are out-of-sync";
            var missingInInts = imps.Except(ints);

            if (missingInInts.Any())
            {
                msg += "\nMissing on Interface: " + string.Join(", ", missingInInts);
            }

            return msg;
        }
    }
}
