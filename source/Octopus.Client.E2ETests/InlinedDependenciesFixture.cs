using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.E2ETests
{
    /// <summary>
    /// These tests are designed to ensure that we keep our published artifact working in the way we expect it to
    ///  * is a single dll that can be referenced from powershell
    ///  * actually works
    /// If you find yourself changing these tests, it's very likely you'll need to change
    /// https://github.com/OctopusDeploy/docs/blob/master/docs/octopus-rest-api/octopus.client.md 
    /// </summary>
    [TestFixture]
    public class InlinedDependenciesFixture
    {
        private Assembly assembly;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var runtime = TestHelpers.GetRuntime();
            var package = TestHelpers.GetNuGetPackage();

            using (var zipFile = ZipFile.OpenRead(package))
            {
                var zipArchiveEntry =
                    zipFile.Entries.FirstOrDefault(x => x.FullName.Contains("lib/" + runtime + "/Octopus.Client.dll"));
                using (var stream = zipArchiveEntry.Open())
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);

                        var rawAssembly = ms.ToArray();
                        assembly = Assembly.Load(rawAssembly);
                    }
                }
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {

        }

        [Test]
        [TestCase("Newtonsoft.Json", "Newtonsoft.Json.JsonConvert")]
        [TestCase("Octodiff", "Octodiff.Core.DeltaBuilder")]
        [TestCase("Octopus.TinyTypes", "Octopus.TinyTypes.CaseInsensitiveStringTinyType")]
        [TestCase("Octopus.TinyTypes.Json", "TinyTypes.Json.TinyTypeJsonConverter")]
        [TestCase("Octopus.TinyTypes.TypeConverters", "Octopus.TinyTypes.TypeConverters.TinyTypeConverters")]
        public void HasInlinedDependency(string library, string typeName)
        {
            assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == typeName)
                .Should()
                .NotBeNull($"We should have ilmerged {library} in, so customers dont need to reference that separately");
        }
    }
}
