using NUnit.Framework;
using OctopusTools.Commands;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class OptionsFixture
    {
        [Test]
        [TestCase("--apiKey=abc123", "-foo=bar")]
        [TestCase("--apikey=abc123", "/foo=bar")]
        [TestCase("-apikey=abc123", "--foo=bar")]
        [TestCase("--apikey=abc123", "-foo=bar")]
        [TestCase("/apikey=abc123", "--foo=bar")]
        public void ShouldBeCaseInsensitive(string parameter1, string parameter2)
        {
            var apiKey = string.Empty;
            var foo = string.Empty;
            
            var optionSet = new OptionSet()
            {
                {"apiKey=", "API key", v => apiKey = v},
                {"foo=", "Foo", v => foo = v}
            };

            optionSet.Parse(new[] {parameter1, parameter2});

            Assert.That(apiKey, Is.EqualTo("abc123"));
            Assert.That(foo, Is.EqualTo("bar"));
        }
    }
}