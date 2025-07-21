using System;
using NUnit.Framework;
using Octopus.Client.Extensions;
using Octopus.TinyTypes;

namespace Octopus.Client.Tests.Extensions
{
    public class TypeExtensionMethodsFixture
    {
        [Test]
        [TestCase(typeof(SomeInt), typeof(TinyType<>), ExpectedResult = true)]
        [TestCase(typeof(TinyType<int>), typeof(TinyType<>), ExpectedResult = true)]
        [TestCase(typeof(int), typeof(TinyType<>), ExpectedResult = false)]
        public bool IsClosedTypeOfOpenGenericShouldBe(Type candidateType, Type openGenericType)
        {
            return candidateType.IsClosedTypeOfOpenGeneric(openGenericType);
        }

        class SomeInt : TinyType<int>
        {
            public SomeInt(int value) : base(value)
            {
            }
        }
    }
}