using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    /// <summary>
    /// Support for reading and writing JSON, exposed for convenience of those using JSON.NET.
    /// </summary>
    public static class JsonSerialization
    {
        /// <summary>
        /// The serializer settings used by Octopus when reading and writing JSON from the
        /// Octopus Deploy RESTful API.
        /// </summary>
        public static JsonSerializerSettings GetDefaultSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new JsonConverterCollection
                {
                    new StringEnumConverter(),
                    new IsoDateTimeConverter {DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK"},
                    new ControlConverter(),
                    new EndpointConverter(),
                    new AccountConverter(),
                    new FeedConverter(),
                    new HrefConverter(null),
                    new PropertyValueResource.PropertyValueJsonConverter(),
                    new TriggerActionConverter(),
                    new TriggerFilterConverter()
                }
            };
        }

        /// <summary>
        /// Serializes the object using the default Octopus.Client serializer
        /// </summary>
        public static string SerializeObject(object obj) => JsonConvert.SerializeObject(obj, GetDefaultSerializerSettings());

        /// <summary>
        /// Deserializes the object using the default Octopus.Client serializer
        /// </summary>
        public static T DeserializeObject<T>(string value) => JsonConvert.DeserializeObject<T>(value, GetDefaultSerializerSettings());
    }
}