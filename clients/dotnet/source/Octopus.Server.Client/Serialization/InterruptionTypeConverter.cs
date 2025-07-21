#nullable enable
using System;
using Newtonsoft.Json;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class InterruptionTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) 
            => objectType == typeof(InterruptionType);

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var v = value.ToString();
            writer.WriteValue(v);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return reader.TokenType switch
            {
                JsonToken.Null => null,
                JsonToken.String => new InterruptionType(reader.Value!.ToString()),
                _ => throw new Exception("Unable to parse token type " + reader.TokenType + " as interruption type string")
            };
        }
    }
}