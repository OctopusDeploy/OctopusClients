using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Extensions;
using Octopus.Client.Model.Forms;

namespace Octopus.Client.Serialization
{
    /// <summary>
    /// Serializes <see cref="Control" />s by including and reading a custom Type property.
    /// </summary>
    class ControlConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var typeName = value.GetType().Name;
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(typeName);

            foreach (var property in value.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty))
            {
                writer.WritePropertyName(property.Name);
                serializer.Serialize(writer, property.GetValue(value, null));
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var typeName = jo.GetValue("Type").ToObject<string>();
            var fullName = typeof (Control).Namespace + "." + typeName;
            var type = typeof (Control).Assembly.GetType(fullName);
            var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var args = from p in ctor.GetParameters()
                let name = char.ToUpper(p.Name[0]) + p.Name.Substring(1)
                let jToken = jo.GetValue(name)
                select jToken == null ? p.ParameterType.GetDefault() : jToken.ToObject(p.ParameterType, serializer);

            var instance = ctor.Invoke(args.ToArray());
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance))
            {
                var jToken = jo.GetValue(prop.Name);
                if(jToken != null)
                    prop.SetValue(instance, jToken.ToObject(prop.PropertyType, serializer), null);
            }
            return instance;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Control).IsAssignableFrom(objectType);
        }
    }
}