using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Model;
using Octopus.TinyTypes.TypeConverters;

namespace Octopus.Client.Serialization
{
    public abstract class InheritedClassConverter<TBaseResource, TEnumType> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var property in value.GetType().GetTypeInfo()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(p => p.CanRead))
            {
                writer.WritePropertyName(property.Name);
                serializer.Serialize(writer, property.GetValue(value, null));
            }

            writer.WriteEndObject();
        }

        protected virtual Type DefaultType { get; } = null;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jo = JObject.Load(reader);
            var designatingProperty = jo.GetValue(TypeDesignatingPropertyName);
            Type type;
            if (designatingProperty == null)
            {
                if (DefaultType == null)
                {
                    throw new Exception($"Unable to determine type to deserialize. Missing property `{TypeDesignatingPropertyName}`");
                }
                type = DefaultType;
            }
            else
            {

                var derivedType = designatingProperty.ToObject<string>();
                TEnumType discriminatingType;
                if (typeof(TEnumType).IsEnum)
                {
                    discriminatingType = (TEnumType)Enum.Parse(typeof(TEnumType), derivedType);
                }
                else
                {
                    discriminatingType = (TEnumType)new TinyTypeConverter<TEnumType>().ConvertFrom(derivedType);
                }
                
                if (!DerivedTypeMappings.ContainsKey(discriminatingType))
                {
                    throw new Exception($"Unable to determine type to deserialize. {TypeDesignatingPropertyName} `{discriminatingType}` does not map to a known type");
                }

                type = DerivedTypeMappings[discriminatingType];
            }

            var ctor = type.GetTypeInfo().GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var args = ctor.GetParameters().Select(p =>
                jo.GetValue(char.ToUpper(p.Name[0]) + p.Name.Substring(1))
                    .ToObject(p.ParameterType, serializer)).ToArray();
            var instance = ctor.Invoke(args);
            foreach (var prop in type
                .GetTypeInfo()
                .GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance)
                .Where(p => p.CanWrite))
            {
                var val = jo.GetValue(prop.Name);
                if (val != null)
                    prop.SetValue(instance, val.ToObject(prop.PropertyType, serializer), null);
            }
            return instance;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TBaseResource).GetTypeInfo().IsAssignableFrom(objectType);
        }

        protected abstract IDictionary<TEnumType, Type> DerivedTypeMappings { get; }

        protected abstract string TypeDesignatingPropertyName { get; }
    }

}