using System;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Text;

namespace Octopus.Client.Serialization
{
    /// <summary>
    /// The supplied Instant converter is very strict, however Octopus is more flexible with the format
    /// it already gets for DateTimeOffset and this should extend to Instant as well
    /// </summary>
    public class PermissiveInstantJsonConverter : JsonConverter<Instant?>
    {
        public override void WriteJson(JsonWriter writer, Instant? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(InstantPattern.ExtendedIso.Format(value.Value));
            }
        }

        public override Instant? ReadJson(JsonReader reader, Type objectType, Instant? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            // Newtonsoft should already have parsed it
            if (reader.TokenType == JsonToken.Date)
            {
                return reader.Value switch
                {
                    DateTimeOffset dateTimeOffset => dateTimeOffset.ToInstant(),
                    DateTime dateTime => new DateTimeOffset(dateTime).ToInstant(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            if (reader.TokenType == JsonToken.String)
            {
                return DateTimeOffset.Parse((string) reader.Value).ToInstant();
            }

            throw new Exception("Unable to parse token type " + reader.TokenType + " as Instant");
        }
    }
}