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
    /// If you find yourself changing these tests, it's very likely you'll need to change
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
            var artifactsFolder = Path.GetFullPath(Path.Combine(path, @"..\..\..\..\..\artifacts"));
            var package = Directory.EnumerateFiles(artifactsFolder , "Octopus.Client.*.nupkg").FirstOrDefault();
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
            
            var sourceFolder = Path.GetFullPath(Path.Combine(path, @"..\..\..\..\..\source\Octopus.Client"));
            var projectFile = Directory.EnumerateFiles(sourceFolder , "Octopus.Client.csproj").FirstOrDefault();
            if (projectFile == null)
                Assert.Fail($"Couldn't find built c# project file with name 'Octopus.Client.csproj' at '{artifactsFolder }'");
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
            dependencyGroups.Count.Should().Be(2, "Should have 2 dependency groups");
        }

        [Test]
        public void NuSpecFileShouldHaveANetFrameworkDependencyGroup()
        {
            var net45DependencyGroup = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETFramework4.5']",
                nameSpaceManager);
            net45DependencyGroup.Should().NotBeNull("Should have a .NETFramework4.5 dependency group");
        }
        
        [Test]
        public void NuSpecFileShouldHaveNoNetFrameworkDependencies()
        {
            var net45Dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETFramework4.5']/ns:dependency",
                nameSpaceManager);
            net45Dependencies.Count.Should().Be(0, "There should be no dependencies listed for .NETFramework4.5");
        }

        [Test]
        public void NuSpecFileShouldHaveANetStandardDependencyGroup()
        {
            var netStandardDependencyGroup = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']",
                nameSpaceManager);
            netStandardDependencyGroup.Should().NotBeNull("Should have a .NETStandard2.0 dependency group");
        }

        [Test]
        public void NuSpecFileShouldHave4NetStandardDependencies()
        {
            var net45Dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency",
                nameSpaceManager);
            net45Dependencies.Count.Should().Be(4, "There should be 4 dependencies listed for .NETStandard2.0");
        }

        [Test]
        public void NuSpecFileShouldHaveNetStandardDependencyOnMicrosoftCSharp()
        {
            var dependency = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Microsoft.CSharp']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a Microsoft.CSharp dependency for .NETStandard2.0");
        }

        [Test]
        public void NuSpecFileShouldHaveNetStandardDependencyOnSystemComponentModelAnnotations()
        {
            var dependency = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='System.ComponentModel.Annotations']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a System.ComponentModel.Annotations dependency for .NETStandard2.0");
        }
        
        [Test]
        public void NuSpecFileShouldHaveNetStandardDependencyOnNewtonsoftJson()
        {
            var dependency = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Newtonsoft.Json']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a Newtonsoft.Json dependency for .NETStandard2.0, as we cant inline it until we get to netcore 3");
        }
        
        [Test]
        public void NuSpecFileShouldHaveNetStandardDependencyOnOctodiff()
        {
            var dependency = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Octodiff']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a Octodiff dependency for .NETStandard2.0, as we cant inline it until we get to netcore 3");
        }

        [Test]
        public void NetStandardDependencyOnNewtonsoftJsonShouldBeSameVersionAsCsProj()
        {
            var actualVersion = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Newtonsoft.Json']/@version",
                nameSpaceManager);
            var expectedVersion = csProjFile.SelectSingleNode("//Project/ItemGroup/PackageReference[@Include='Newtonsoft.Json']/@Version");
            actualVersion.Value.Should()
                .Be(expectedVersion.Value, "The Newtonsoft.Json dependency version in the nuspec should match the one in the csproj");
        }
        
        [Test]
        public void NetStandardDependencyOnOctodiffShouldBeSameVersionAsCsProj()
        {
            var actualVersion = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Octodiff']/@version",
                nameSpaceManager);
            var expectedVersion = csProjFile.SelectSingleNode("//Project/ItemGroup/PackageReference[@Include='Octodiff']/@Version");
            actualVersion.Value.Should()
                .Be(expectedVersion.Value, "The Octodiff dependency version in the nuspec should match the one in the csproj");
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
            var nonNet45Dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework != '.NETFramework4.5']",
                nameSpaceManager);
            nonNet45Dependencies.Count.Should().Be(0, "There should be only be frameworkAssemblies listed for .NETFramework4.5");
        }

        [Test]
        public void NuSpecFileShouldHave3FrameworkAssembliesForNetFramework()
        {
            var net45Dependencies = nuSpecFile.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5']",
                nameSpaceManager);
            net45Dependencies.Count.Should().Be(3, "There should be 3 frameworkAssemblies listed for .NETFramework4.5");
        }
        
        [Test]
        public void NuSpecFileShouldHaveSystemComponentModelDataAnnotationsFrameworkAssemblyForNetFramework()
        {
            var frameworkAssembly = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5' and @assemblyName = 'System.ComponentModel.DataAnnotations']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull("Should have a frameworkAssembly node for 'System.ComponentModel.DataAnnotations' for .NETFramework4.5");
        }
        
        [Test]
        public void NuSpecFileShouldHaveSystemNetHttpFrameworkAssemblyForNetFramework()
        {
            var frameworkAssembly = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5' and @assemblyName = 'System.Net.Http']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull("Should have a frameworkAssembly node for 'System.Net.Http' for .NETFramework4.5");
        }
        
        [Test]
        public void NuSpecFileShouldHaveSystemNumericsFrameworkAssemblyForNetFramework()
        {
            var frameworkAssembly = nuSpecFile.SelectSingleNode(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5' and @assemblyName = 'System.Numerics']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull("Should have a frameworkAssembly node for 'System.Numerics' for .NETFramework4.5");
        }
    }
}
