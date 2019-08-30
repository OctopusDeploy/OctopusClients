using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.E2ETests
{
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
#if SUPPORTS_ILMERGE
        [Test]
        public void HasInlinedNewtonsoftJson()
        {
            assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == "Newtonsoft.Json.JsonConvert")
                .Should()
                .NotBeNull(
                    "We should have ilmerged Newtonsoft.Json in, so customers dont need to reference that separately");
        }

        [Test]
        public void HasInlinedOctodiff()
        {
            assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == "Octodiff.Core.DeltaBuilder")
                .Should()
                .NotBeNull("We should have ilmerged Octodiff in, so customers dont need to reference that separately");
        }
#else
        [Test]
        public void HasNotInlinedNewtonsoftJson()
        {
            assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == "Newtonsoft.Json.JsonConvert")
                .Should()
                .BeNull("We aren't able to ilmerge Newtonsoft.Json in yet as it's only available from netcore 3");
        }

        [Test]
        public void HasNotInlinedOctodiff()
        {
            assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == "Octodiff.Core.DeltaBuilder")
                .Should()
                .BeNull("We aren't able to ilmerge Octodiff in yet as it's only available from netcore 3");
        }
#endif
    }
}
