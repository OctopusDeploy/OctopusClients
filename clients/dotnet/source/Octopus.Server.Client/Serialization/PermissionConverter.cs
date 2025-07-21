#nullable enable
using System;
using Newtonsoft.Json;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class PermissionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Permission);
        }

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
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new Permission(reader.Value!.ToString());
            }

            throw new Exception("Unable to parse token type " + reader.TokenType + " as permission string");
        }
    }
}