using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Serialization;
using Octopus.TinyTypes;
using Octopus.TinyTypes.TypeConverters;

namespace Octopus.Client.Tests.TypeConversionTests
{
    public class TinyTypeTypeConverterRegistrationFixture
    {
        [Test]
        public void WhenCreatingASynchronousClient_TinyTypeConvertersShouldBeRegistered()
        {
            // Just fire the static type initializer. We don't need to use anything it generates; we just need it to fire.
            JsonSerialization.GetDefaultSerializerSettings();

            var converter = TypeDescriptor.GetConverter(typeof(SomeTypedInt));

            converter.Should().BeOfType<TinyTypeConverter<SomeTypedInt>>();
        }

        private class SomeTypedInt : TinyType<int>
        {
            public SomeTypedInt(int value) : base(value)
            {
            }
        }
    }
}