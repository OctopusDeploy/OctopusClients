#nullable enable
using System;
using Newtonsoft.Json;
using NodaTime;
using Octopus.Client.Extensions;

namespace Octopus.Client.Serialization;

public class CrossPlatformDateTimeZoneJsonConverter : JsonConverter<DateTimeZone?>
{
    public override void WriteJson(JsonWriter writer, DateTimeZone? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(value.ToWindowsTimeZoneId());
        }
    }

    public override DateTimeZone? ReadJson(JsonReader reader, Type objectType, DateTimeZone? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.String && reader.Value != null)
        {
            return ((string)reader.Value).ToDateTimeZone();
        }

        throw new Exception("Unable to parse token type " + reader.TokenType + " as a Time Zone");
    }
}