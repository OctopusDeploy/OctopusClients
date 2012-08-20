using NUnit.Framework;
using OctopusTools.Commands;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class OptionsFixture
    {
        [Test]
        [TestCase("--apiKey=abc123")]
        [TestCase("--apikey=abc123")]
        [TestCase("-apikey=abc123")]
        [TestCase("--apikey=abc123")]
        [TestCase("/apikey=abc123")]
        public void ShouldBeCaseInsensitive(string parameter)
        {
            var apiKey = string.Empty;
            var optionSet = new OptionSet()
            {
                {"apiKey=", "API key", v => apiKey = v}
            };

            optionSet.Parse(new[] {parameter});

            Assert.That(apiKey, Is.EqualTo("abc123"));
        }
    }
}