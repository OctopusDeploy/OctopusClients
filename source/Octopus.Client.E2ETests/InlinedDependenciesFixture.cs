using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable ReplaceWithSingleCallToFirstOrDefault

namespace Octopus.Client.E2ETests
{
    /// <summary>
    ///     These tests are designed to ensure that we keep our published artifact working in the way we expect it to
    ///     * is a single dll that can be referenced from powershell
    ///     * actually works
    ///     If you find yourself changing these tests, it's very likely you'll need to change
    ///     https://github.com/OctopusDeploy/docs/blob/master/docs/octopus-rest-api/octopus.client.md
    /// </summary>
    [TestFixture]
    public class InlinedDependenciesFixture
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var runtime = TestHelpers.GetRuntime();
            var package = TestHelpers.GetNuGetPackage();

            using var zipFile = ZipFile.OpenRead(package);
            var zipArchiveEntry = zipFile.Entries.First(x => x.FullName.Contains("lib/" + runtime + "/Octopus.Client.dll"));
            using var stream = zipArchiveEntry.Open();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            var rawAssembly = ms.ToArray();
            assembly = Assembly.Load(rawAssembly);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }

        private Assembly assembly;

        [Test]
        [TestCase("Newtonsoft.Json", "Newtonsoft.Json.JsonConvert", Visibility.Internal)]
        [TestCase("Octodiff", "Octodiff.Core.DeltaBuilder", Visibility.Internal)]
        [TestCase("Octopus.TinyTypes", "Octopus.TinyTypes.TinyType`1", Visibility.Public)]
        [TestCase("Octopus.TinyTypes.Json", "Octopus.TinyTypes.Json.TinyTypeJsonConverter", Visibility.Internal)]
        [TestCase("Octopus.TinyTypes.TypeConverters", "Octopus.TinyTypes.TypeConverters.TinyTypeConverter`1", Visibility.Internal)]
        [TestCase("Octopus.Server.MessageContracts.Base", "Octopus.Server.MessageContracts.Base.ICommand`2", Visibility.Public)]
        [TestCase("Octopus.Server.MessageContracts.Base.HttpRoutes", "Octopus.Server.MessageContracts.Base.HttpRoutes.HttpRouteTemplateAttribute", Visibility.Public)]
        public void HasInlinedDependency(string library, string typeName, Visibility expectedVisibility)
        {
            var type = assembly.GetTypes()
                .Where(t => t.FullName == typeName)
                .FirstOrDefault();

            type
                .Should()
                .NotBeNull(
                    $"We should have ILMerged {library} in, so customers don't need to reference that separately.");

            // Some of our dependencies should remain public. Some should be internalized when we do the il-repack step.
            switch (expectedVisibility)
            {
                case Visibility.Internal:
                    type!.IsPublic.Should().BeFalse();
                    break;
                case Visibility.Public:
                    type!.IsPublic.Should().BeTrue();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedVisibility), expectedVisibility, null);
            }
        }

        public enum Visibility
        {
            Internal,
            Public
        }
    }
}