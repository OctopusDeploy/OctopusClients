using NUnit.Framework;
using Octopus.Client.Validation;

namespace Octopus.Client.Tests.Model
{
    public class PasswordComplexityAttributeFixture
    {
        [Test]
        [TestCase("", true)] // Special case because empty strings are caught by the [Required] rule
        [TestCase("a", false)]
        [TestCase("abc", false)]
        [TestCase("abdefgh", false)]
        [TestCase("password", false)]
        [TestCase("Password", false)]
        [TestCase("Password01", true)]
        [TestCase("Password01!", true)]
        [TestCase("Tr0ub4dor&3", true)]
        [TestCase("correct horse battery stable", true)]
        public void ShouldPassOrFail(string password, bool shouldBeValid)
        {
            var attribute = new PasswordComplexityAttribute();
            var wasValid = attribute.IsValid(password);

            Assert.That(wasValid, Is.EqualTo(shouldBeValid));
        }
    }
}