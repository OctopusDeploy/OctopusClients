using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Octopus.Client.Serialization
{
    /// <summary>
    /// Multiple dates are required to be parseable to allow the new client
    /// (which now models the package publish date property as a DateTimeOffset instead of a string)
    /// to be both forwards and backwards compatible with the new format being returned
    /// https://github.com/OctopusDeploy/Issues/issues/5535 
    /// </summary>
    public class MultiIsoDateTimeFormatConverter : JsonConverter
    {
        public MultiIsoDateTimeFormatConverter(string defaultFormat, params string[] additionalReadFormats)
        {
            converters = new List<JsonConverter>();
            converters.Add(new IsoDateTimeConverter(){ DateTimeFormat = defaultFormat });
            converters.AddRange(additionalReadFormats.Select(format => new IsoDateTimeConverter() {DateTimeFormat = format}));
        }
        private readonly List<JsonConverter> converters;
            
        private JsonConverter DefaultConverter => converters[0];
            
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DefaultConverter.WriteJson(writer, value, serializer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Try all converters catching any errors, then let any errors from the last converter bubble up
            for (var i = 0; i < converters.Count; i++)
            {
                try
                {
                    return converters[i].ReadJson(reader, objectType, existingValue, serializer);
                }
                catch
                {
                    // Try next converter
                }
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return DefaultConverter.CanConvert(objectType);
        }
    }
}