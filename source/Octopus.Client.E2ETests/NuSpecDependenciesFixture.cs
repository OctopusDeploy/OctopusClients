using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.E2ETests
{
    /// <summary>
    /// These tests are designed to ensure that we keep our published artifact working in the way we expect it to
    ///  * is a single dll that can be referenced from powershell
    ///  * inlines dependencies properly
    ///  * doesn't pull down unnecessary dependencies
    /// If you find yourself changing these tests, it's very likely you'll need to change build.cs and
    /// https://github.com/OctopusDeploy/docs/blob/master/docs/octopus-rest-api/octopus.client.md
    /// </summary>
    [TestFixture]
    public class NuSpecDependenciesFixture
    {
        private XmlDocument nuSpecFile;
        private XmlDocument csProjFile;
        private XmlNamespaceManager nameSpaceManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullLocalPath());
            var artifactsFolder = Path.GetFullPath(Path.Combine(path, "..", "..", "..", "..", "..", "artifacts"));
            var package = Directory.EnumerateFiles(artifactsFolder, "Octopus.Client.*.nupkg").FirstOrDefault();
            if (package == null)
                Assert.Fail($"Couldn't find built nuget package with name 'Octopus.Client.*.nupkg' at '{artifactsFolder }'");
            using (var zipFile = ZipFile.OpenRead(package))
            {
                nuSpecFile = new XmlDocument();
                var zipArchiveEntry = zipFile.Entries.FirstOrDefault(x => x.Name == "Octopus.Client.nuspec");
                if (zipArchiveEntry == null)
                    Assert.Fail($"Unable to find 'Octopus.Client.nuspec' in the nupkg file.");

                using (var stream = zipArchiveEntry.Open())
                {
                    nameSpaceManager = new XmlNamespaceManager(nuSpecFile.NameTable);
                    nameSpaceManager.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");

                    nuSpecFile.Load(stream);
                }
            }

            // Yep. We're pulling the source from a different location (the Octopus.Server.Client project) as it's the one
            // which actually has all of the relevant package references.
            var sourceFolder = Path.GetFullPath(Path.Combine(path, "..", "..", "..", "..", "..", "source", "Octopus.Server.Client"));
            var projectFile = Directory.EnumerateFiles(sourceFolder, "Octopus.Server.Client.csproj").FirstOrDefault();
            if (projectFile == null)
                Assert.Fail($"Couldn't find built c# project file with name 'Octopus.Server.Client.csproj' at '{artifactsFolder }'");
            csProjFile = new XmlDocument();
            csProjFile.Load(projectFile);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {

        }

        [Test]
        public void NuSpecFileShouldHaveADependencyNode()
        {
            var dependencyNode = nuSpecFile.SelectSingleNode("//ns:package/ns:metadata/ns:dependencies", nameSpaceManager);
            dependencyNode.Should().NotBeNull("We should have a dependencies node");
        }

        [Test]
        public void NuSpecFileShouldHaveADependencyGroupForEachTargetFramework()
        {
            var dependencyGroups = nuSpecFile.SelectNodes("//ns:package/ns:metadata/ns:dependencies/ns:group", nameSpaceManager);
            dependencyGroups.Count.Should().Be(3, "Should have 3 dependency groups");
        }

        public static TestCaseData[] DependencyExpectations =
        {
            //if you need to change this, you'll probably need to change the Build.cs
            new TestCaseData(".NETFramework4.6.2",
                new string[] {
                }),
            new TestCaseData(".NETStandard2.0",
                new[]
                {
                    "Microsoft.CSharp",
                    "System.ComponentModel.Annotations"
                }),
        };

        public static IEnumerable<TestCaseData> DependencyExpectationsFlattened()
        {
            foreach (var group in DependencyExpectations)
                foreach (var dep in (string[])group.Arguments[1])
                    yield return new TestCaseData(group.Arguments[0], dep);
        }

        [Test]
        [TestCaseSource(nameof(DependencyExpectations))]
        public void NuSpecFileShouldHaveCorrectDependencyGroups(string framework, string[] dependencies)
        {
            var dependencyGroupNode = nuSpecFile.SelectSingleNode(
                $"//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '{framework}']",
                nameSpaceManager);
            dependencyGroupNode.Should().NotBeNull($"Should have a {framework} dependency group");
        }

        [Test]
        [TestCaseSource(nameof(DependencyExpectations))]
        public void NuSpecFileShouldHaveCorrectDependencyCounts(string framework, string[] dependencies)
        {
            var dependencyNodes = nuSpecFile.SelectNodes(
                $"//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '{framework}']/ns:dependency",
                nameSpaceManager);
            dependencyNodes.Count.Should().Be(dependencies.Length, $"There should be {dependencies.Length} dependencies listed for {framework}");
        }

        [Test]
        [TestCaseSource(nameof(DependencyExpectationsFlattened))]
        public void NuSpecFileShouldHaveExpectedDependency(string framework, string dependency)
        {
            var node = nuSpecFile.SelectSingleNode(
                $"//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '{framework}']/ns:dependency[@id='{dependency}']",
                nameSpaceManager);
            node.Should()
                .NotBeNull($"There should be a {dependency} dependency for {framework}");
        }

        [Test]
        [TestCaseSource(nameof(DependencyExpectationsFlattened))]
        public void DependencyVersionShouldBeSameVersionAsCsProj(string framework, string dependency)
        {
            var actualVersion = nuSpecFile.SelectSingleNode(
                $"//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '{framework}']/ns:dependency[@id='{dependency}']/@version",
                nameSpaceManager);
            actualVersion.Should().NotBeNull();

            var expectedVersion = csProjFile.SelectSingleNode($"//Project/ItemGroup/PackageReference[@Include='{dependency}']/@Version");
            expectedVersion.Should().NotBeNull();

            actualVersion!.Value.Should().Be(expectedVersion!.Value, $"The {dependency} dependency version in the nuspec should match the one in the csproj");
        }

        [Test]
        public void NuSpecFileShouldHaveAFrameworkAssembliesNode()
        {
            var node = nuSpecFile.SelectSingleNode("//ns:package/ns:metadata/ns:frameworkAssemblies", nameSpaceManager);
            node.Should().NotBeNull("We should have a frameworkAssemblies node");
        }

        [Test]
        public void NuSpecFileShouldOnlyHaveFrameworkAssembliesForNetFramework()
        {
            var dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework != '.NETFramework4.6.2' and @targetFramework != '.NETFramework4.8']",
                nameSpaceManager);
            dependencies.Count.Should().Be(0, "There should be only be frameworkAssemblies listed for .NETFramework4.6.2 and .NETFramework4.8");
        }

        [Test]
        public void NuSpecFileShouldHave3FrameworkAssembliesForNetFramework462()
        {
            var dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.6.2']",
                nameSpaceManager);
            dependencies.Count.Should().Be(3, "There should be 3 frameworkAssemblies listed for .NETFramework4.6.2");
        }

        [Test]
        public void NuSpecFileShouldHave3FrameworkAssembliesForNetFramework48()
        {
            var dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.8']",
                nameSpaceManager);
            dependencies.Count.Should().Be(3, "There should be 3 frameworkAssemblies listed for .NETFramework4.8");
        }

        [Test]
        [TestCase("System.ComponentModel.DataAnnotations")]
        [TestCase("System.Net.Http")]
        [TestCase("System.Numerics")]
        public void NuSpecFileShouldHaveSystemComponentModelDataAnnotationsFrameworkAssemblyForNetFramework462(string assemblyName)
        {
            var frameworkAssembly = nuSpecFile.SelectSingleNode(
                $"//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.6.2' and @assemblyName = '{assemblyName}']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull($"Should have a frameworkAssembly node for '{assemblyName}' for .NETFramework4.6.2");
        }

        [Test]
        [TestCase("System.ComponentModel.DataAnnotations")]
        [TestCase("System.Net.Http")]
        [TestCase("System.Numerics")]
        public void NuSpecFileShouldHaveSystemComponentModelDataAnnotationsFrameworkAssemblyForNetFramework48(string assemblyName)
        {
            var frameworkAssembly = nuSpecFile.SelectSingleNode(
                $"//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.8' and @assemblyName = '{assemblyName}']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull($"Should have a frameworkAssembly node for '{assemblyName}' for .NETFramework4.8");
        }
    }
}
