using System;

using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class HrefConverter : JsonConverter
    {
        readonly Func<string> baseUrlResolver;

        public HrefConverter(Func<string> baseUrlResolver)
        {
            this.baseUrlResolver = baseUrlResolver;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var link = ((Href)value).AsString();
            if (link == null)
            {
                writer.WriteNull();
                return;
            }

            if (link.StartsWith("~/"))
            {
                link = link.TrimStart('~', '/');

                if (baseUrlResolver == null)
                {
                    throw new Exception("No base URL resolver is configured for the HrefConverter, so the application-relative path in " + link + " cannot be resolved");
                }

                var localBaseUrl = baseUrlResolver();
                if (string.IsNullOrWhiteSpace(localBaseUrl))
                {
                    link = "/" + link;
                }
                else
                {
                    link = "/" + localBaseUrl.Trim('/') + "/" + link;
                }
            }

            writer.WriteValue(link);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new Href(reader.Value.ToString());
            }

            throw new Exception("Unable to parse token type " + reader.TokenType + " as string");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Href);
        }
    }
}