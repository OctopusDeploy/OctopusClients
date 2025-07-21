using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Octopus.Client.Serialization;
using Octopus.TinyTypes;

namespace Octopus.Client.Tests.Serialization
{
    [TestFixture]
    public class InheritedClassConverterFixture
    {
        [Test]
        public void StringDiscriminator()
        {
            var input = new
            {
                Discriminator = StringDiscriminatorClass1.DiscriminatorName
            };

            var result = Execute<StringDiscriminatorBaseClass>(input, new StringDiscriminatorClassConverter());

            result.Should().BeOfType<StringDiscriminatorClass1>();
        }

        [Test]
        public void EnumDiscriminator()
        {
            var input = new
            {
                Discriminator = nameof(DiscriminatorEnum.Class1)
            };

            var result = Execute<EnumDiscriminatorBaseClass>(input, new EnumDiscriminatorClassConverter());

            result.Should().BeOfType<EnumDiscriminatorClass1>();
        }

        [Test]
        public void TinyTypeDiscriminator()
        {
            var input = new
            {
                Discriminator = DiscriminatorTinyType.Class1Name.Value
            };

            var result = Execute<TinyTypeDiscriminatorBaseClass>(input, new TinyTypeDiscriminatorClassConverter());

            result.Should().BeOfType<TinyTypeDiscriminatorClass1>();
        }
        
        private static T Execute<T>(object input, JsonConverter customJsonConverter)
        {
            //Serialize anonymous object to JSON
            var json = JsonConvert.SerializeObject(input);
            
            var settings = JsonSerialization.GetDefaultSerializerSettings();
            settings.Converters.Add(customJsonConverter);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        abstract class StringDiscriminatorBaseClass
        {
            public abstract string Discriminator { get; }
        }

        class StringDiscriminatorClass1 : StringDiscriminatorBaseClass
        {
            public const string DiscriminatorName = "Class1";
            public override string Discriminator => DiscriminatorName;
        }
        
        class StringDiscriminatorClassConverter : InheritedClassConverter<StringDiscriminatorBaseClass, string>
        {
            static readonly IDictionary<string, Type> TypeMappings =
                new Dictionary<string, Type>
                {
                    {StringDiscriminatorClass1.DiscriminatorName, typeof(StringDiscriminatorClass1)}
                };

            protected override IDictionary<string, Type> DerivedTypeMappings => TypeMappings;
            protected override string TypeDesignatingPropertyName => "Discriminator";
        }

        enum DiscriminatorEnum
        {
            Something,
            Class1
        }

        abstract class EnumDiscriminatorBaseClass
        {
            public abstract DiscriminatorEnum Discriminator { get; }
        }

        class EnumDiscriminatorClass1 : EnumDiscriminatorBaseClass
        {
            public const DiscriminatorEnum DiscriminatorName = DiscriminatorEnum.Class1;
            public override DiscriminatorEnum Discriminator => DiscriminatorName;
        }
        
        class EnumDiscriminatorClassConverter : InheritedClassConverter<EnumDiscriminatorBaseClass, DiscriminatorEnum>
        {
            static readonly IDictionary<DiscriminatorEnum, Type> TypeMappings =
                new Dictionary<DiscriminatorEnum, Type>
                {
                    {DiscriminatorEnum.Class1, typeof(EnumDiscriminatorClass1)}
                };

            protected override IDictionary<DiscriminatorEnum, Type> DerivedTypeMappings => TypeMappings;
            protected override string TypeDesignatingPropertyName => "Discriminator";
        }
        
        class DiscriminatorTinyType : CaseInsensitiveStringTinyType
        {
            public static readonly DiscriminatorTinyType Class1Name = new DiscriminatorTinyType("Class1");
            public DiscriminatorTinyType(string value) : base(value)
            {
            }
        }

        abstract class TinyTypeDiscriminatorBaseClass
        {
            public abstract DiscriminatorTinyType Discriminator { get; }
        }

        class TinyTypeDiscriminatorClass1 : TinyTypeDiscriminatorBaseClass
        {
            public static readonly DiscriminatorTinyType DiscriminatorName = DiscriminatorTinyType.Class1Name;
            public override DiscriminatorTinyType Discriminator => DiscriminatorName;
        }
        
        class TinyTypeDiscriminatorClassConverter : InheritedClassConverter<TinyTypeDiscriminatorBaseClass, DiscriminatorTinyType>
        {
            static readonly IDictionary<DiscriminatorTinyType, Type> TypeMappings =
                new Dictionary<DiscriminatorTinyType, Type>
                {
                    {DiscriminatorTinyType.Class1Name, typeof(TinyTypeDiscriminatorClass1)}
                };

            protected override IDictionary<DiscriminatorTinyType, Type> DerivedTypeMappings => TypeMappings;
            protected override string TypeDesignatingPropertyName => "Discriminator";
        }
    }
}