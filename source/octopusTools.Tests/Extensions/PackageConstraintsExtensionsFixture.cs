using System;
using NUnit.Framework;

namespace OctopusTools.Tests.Extensions
{
    [TestFixture]
    public class PackageConstraintsExtensionsFixture
    {

        [Test]
        public void ShouldReturnFalseForInvalidVersionStrings()
        {
            string[] stringsToTest = {"","  ","a","a.b","1,000","1.0.3-alpha","1.0. "};
            foreach (var s in stringsToTest)
            {
                Assert.IsFalse(PackageConstraintExtensions.IsValidVersionString(s));    
            }
        }

        [Test]
        public void ShouldReturnTrueForValidVersionStrings()
        {
            string[] stringsToTest = {"0","0.1","1.0.0","0.0.0.0","1.2"};
            foreach (var s in stringsToTest)
            {
                Assert.IsTrue(PackageConstraintExtensions.IsValidVersionString(s));
            }

        }

        [Test]
        public void ShouldReturnFalseForInvalidNugetPackageIds()
        {
            // http://nuget.codeplex.com/wikipage?title=Package%20Id%20Specification&IsNewlyCreatedPage=true
            string[] stringsToTest = { "hello._there","i.am.not.valid.","_neitherami","bad..id",""," " };
            foreach (var s in stringsToTest)
            {
                Assert.IsFalse(PackageConstraintExtensions.IsValidNugetPackageId(s));
            }
        }

        [Test]
        public void ShouldReturnTrueForValidNugetPackageIds()
        {
            // http://nuget.codeplex.com/wikipage?title=Package%20Id%20Specification&IsNewlyCreatedPage=true
            string[] stringsToTest = { "hello","i.am.a.valid.id","nuget-core_is.cool","123.456.789","newsletters" };
            foreach (var s in stringsToTest)
            {
                Assert.IsTrue(PackageConstraintExtensions.IsValidNugetPackageId(s));
            }
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionForInvalidPackageConstraint()
        {
            string[] stringsToTest = {"package name:1.0",""," ","packageName:1.0:2.0","packageA:1.0;packageB:2.0"};
            foreach (var s in stringsToTest)
            {
                Assert.Throws<ArgumentException>(delegate
                                                                             {
                                                                                 PackageConstraintExtensions.
                                                                                     GetPackageIdAndVersion(s);
                                                                             });
            }
        }

        [Test]
        public void ShouldReturnTokensForValidPackageConstraint()
        {
            string[] stringsToTest = { "packageName:1", "packageName:1.02.4.0"};
            foreach (var s in stringsToTest)
            {
                var tokens = PackageConstraintExtensions.GetPackageIdAndVersion(s);
                Assert.IsTrue(tokens.Length==2);
                Assert.IsFalse(string.IsNullOrWhiteSpace(tokens[0]));
                Assert.IsFalse(string.IsNullOrWhiteSpace(tokens[1]));
            }

        }
    }
}
