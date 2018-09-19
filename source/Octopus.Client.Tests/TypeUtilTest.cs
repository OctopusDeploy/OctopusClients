using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Util;

namespace Octopus.Client.Tests
{
    interface IAmTheBase<T>
    {
    }

    interface IAmGeneric<T> : IAmTheBase<T>
    {
    }

    class GenericPerson<T> : IAmGeneric<T>
    {
    }

    class Person<T> : IAmTheBase<T>
    {

    }

    [TestFixture]
    public class TypeUtilTests
    {
        [Test]
        public void CanAssignDirectly()
        {
            var genericPerson = new GenericPerson<string>();
            TypeUtil.IsAssignableToGenericType(genericPerson.GetType(), typeof(IAmGeneric<>)).Should().Be(true);
        }

        [Test]
        public void CanAssignIndirectly()
        {
            var genericPerson = new GenericPerson<string>();
            TypeUtil.IsAssignableToGenericType(genericPerson.GetType(), typeof(IAmTheBase<>)).Should().Be(true);
        }

        [Test]
        public void CanNotAssign()
        {
            var normalPerson = new Person<string>();
            TypeUtil.IsAssignableToGenericType(normalPerson.GetType(), typeof(IAmGeneric<>)).Should().Be(false);
        }
    }
}
