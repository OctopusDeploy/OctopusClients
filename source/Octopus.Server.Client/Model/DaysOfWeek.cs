using System;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    [Flags]
    [JsonConverter(typeof(DaysOfWeekFlagConverter))]
    public enum DaysOfWeek
    {
        Sunday = 0x1,
        Monday = 0x2,
        Tuesday = 0x4,
        Wednesday = 0x8,
        Thursday = 0x10,
        Friday = 0x20,
        Saturday = 0x40
    }

    public class DaysOfWeekFlagConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var outVal = 0;
            if (reader.TokenType != JsonToken.StartArray) return outVal;

            reader.Read();
            while (reader.TokenType != JsonToken.EndArray)
            {
                outVal += (int)Enum.Parse(objectType, reader.Value.ToString());
                reader.Read();
            }
            return outVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var flags = value.ToString()
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => $"\"{f}\"");

            writer.WriteRawValue($"[{string.Join(", ", flags)}]");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DaysOfWeek).IsAssignableFrom(objectType);
        }
    }
}
