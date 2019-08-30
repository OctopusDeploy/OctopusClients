using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.E2ETests
{
    [TestFixture]
    public class NuSpecDependenciesFixture
    {
        private XmlDocument xmlDoc;
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
                xmlDoc = new XmlDocument();
                var zipArchiveEntry = zipFile.Entries.FirstOrDefault(x => x.Name == "Octopus.Client.nuspec");
                if (zipArchiveEntry == null)
                    Assert.Fail($"Unable to find 'Octopus.Client.nuspec' in the nupkg file.");
               
                using (var stream = zipArchiveEntry.Open())
                {
                    nameSpaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                    nameSpaceManager.AddNamespace("ns", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");

                    xmlDoc.Load(stream);
                }
            }
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            
        }

        [Test]
        public void NuSpecFileShouldHaveADependencyNode()
        {
            var dependencyNode = xmlDoc.SelectSingleNode("//ns:package/ns:metadata/ns:dependencies", nameSpaceManager);
            dependencyNode.Should().NotBeNull("We should have a dependencies node");
        }

        [Test]
        public void NuSpecFileShouldHaveADependencyGroupForEachTargetFramework()
        {
            var dependencyGroups = xmlDoc.SelectNodes("//ns:package/ns:metadata/ns:dependencies/ns:group", nameSpaceManager);
            dependencyGroups.Count.Should().Be(2, "Should have 2 dependency groups");
        }

        [Test]
        public void NuSpecFileShouldHaveANetFramework45DependencyGroup()
        {
            var net45DependencyGroup = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETFramework4.5']",
                nameSpaceManager);
            net45DependencyGroup.Should().NotBeNull("Should have a .NETFramework4.5 dependency group");
        }
        
        [Test]
        public void NuSpecFileShouldHaveNoNetFramework45Dependencies()
        {
            var net45Dependencies = xmlDoc.SelectNodes(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETFramework4.5']/ns:dependency",
                nameSpaceManager);
            net45Dependencies.Count.Should().Be(0, "There should be no dependencies listed for .NETFramework4.5");
        }

        [Test]
        public void NuSpecFileShouldHaveANetStandard20DependencyGroup()
        {
            var netStandard20DependencyGroup = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']",
                nameSpaceManager);
            netStandard20DependencyGroup.Should().NotBeNull("Should have a .NETStandard2.0 dependency group");
        }

        [Test]
        public void NuSpecFileShouldHave4NetStandard20Dependencies()
        {
            var net45Dependencies = xmlDoc.SelectNodes(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency",
                nameSpaceManager);
            net45Dependencies.Count.Should().Be(4, "There should be 4 dependencies listed for .NETStandard2.0");
        }

        [Test]
        public void NuSpecFileShouldHaveNetStandard20DependencyOnMicrosoftCSharp()
        {
            var dependency = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Microsoft.CSharp']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a Microsoft.CSharp dependency for .NETStandard2.0");
        }

        [Test]
        public void NuSpecFileShouldHaveNetStandard20DependencyOnSystemComponentModelAnnotations()
        {
            var dependency = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='System.ComponentModel.Annotations']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a System.ComponentModel.Annotations dependency for .NETStandard2.0");
        }
        
        [Test]
        public void NuSpecFileShouldHaveNetStandard20DependencyOnNewtonsoftJson()
        {
            var dependency = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Newtonsoft.Json']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a Newtonsoft.Json dependency for .NETStandard2.0, as we cant inline it until we get to netcore 3");
        }
        
        [Test]
        public void NuSpecFileShouldHaveNetStandard20DependencyOnOctodiff()
        {
            var dependency = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:dependencies/ns:group[@targetFramework = '.NETStandard2.0']/ns:dependency[@id='Octodiff']",
                nameSpaceManager);
            dependency.Should()
                .NotBeNull("There should be a Octodiff dependency for .NETStandard2.0, as we cant inline it until we get to netcore 3");
        }

        [Test]
        public void NuSpecFileShouldHaveAFrameworkAssembliesNode()
        {
            var node = xmlDoc.SelectSingleNode("//ns:package/ns:metadata/ns:frameworkAssemblies", nameSpaceManager);
            node.Should().NotBeNull("We should have a frameworkAssemblies node");
        }
        
        [Test]
        public void NuSpecFileShouldOnlyHaveFrameworkAssembliesForNetFramework45()
        {
            var nonNet45Dependencies = xmlDoc.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework != '.NETFramework4.5']",
                nameSpaceManager);
            nonNet45Dependencies.Count.Should().Be(0, "There should be only be frameworkAssemblies listed for .NETFramework4.5");
        }

        [Test]
        public void NuSpecFileShouldHave3FrameworkAssembliesForNetFramework45()
        {
            var net45Dependencies = xmlDoc.SelectNodes(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5']",
                nameSpaceManager);
            net45Dependencies.Count.Should().Be(3, "There should be 3 frameworkAssemblies listed for .NETFramework4.5");
        }
        
        [Test]
        public void NuSpecFileShouldHaveSystemComponentModelDataAnnotationsFrameworkAssemblyForNetFramework45()
        {
            var frameworkAssembly = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5' and @assemblyName = 'System.ComponentModel.DataAnnotations']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull("Should have a frameworkAssembly node for 'System.ComponentModel.DataAnnotations' for .NETFramework4.5");
        }
        
        [Test]
        public void NuSpecFileShouldHaveSystemNetHttpFrameworkAssemblyForNetFramework45()
        {
            var frameworkAssembly = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5' and @assemblyName = 'System.Net.Http']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull("Should have a frameworkAssembly node for 'System.Net.Http' for .NETFramework4.5");
        }
        
        [Test]
        public void NuSpecFileShouldHaveSystemNumericsFrameworkAssemblyForNetFramework45()
        {
            var frameworkAssembly = xmlDoc.SelectSingleNode(
                "//ns:package/ns:metadata/ns:frameworkAssemblies/ns:frameworkAssembly[@targetFramework = '.NETFramework4.5' and @assemblyName = 'System.Numerics']",
                nameSpaceManager);
            frameworkAssembly.Should().NotBeNull("Should have a frameworkAssembly node for 'System.Numerics' for .NETFramework4.5");
        }
    }
}
