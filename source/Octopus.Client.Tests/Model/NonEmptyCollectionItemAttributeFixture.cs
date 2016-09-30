using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Validation;

namespace Octopus.Client.Tests.Model
{
    [TestFixture]
    public class NonEmptyCollectionItemAttributeFixture
    {
        [Test]
        public void EmptyCollectionShouldPass()
        {
            var attribute = new NonEmptyCollectionItemAttribute();
            Assert.IsTrue(attribute.IsValid(new ReferenceCollection()));
        }

        [Test]
        public void CollectionWithBlankStringShouldFail()
        {
            var attribute = new NonEmptyCollectionItemAttribute();
            Assert.IsFalse(attribute.IsValid(new ReferenceCollection(new[] {""})));
        }

        [Test]
        public void CollectionWithSomethingInItShouldPass()
        {
            var attribute = new NonEmptyCollectionItemAttribute();
            Assert.IsTrue(attribute.IsValid(new ReferenceCollection(new[] {"project-1"})));
        }
    }
}