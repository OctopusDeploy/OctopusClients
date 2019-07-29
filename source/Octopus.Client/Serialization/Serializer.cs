using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json;
using Octopus.Cli.Extensions;

namespace Octopus.Client.Serialization
{
    public static class Serializer
    {
        public static string Serialize<T>(object metadata, T exportObject)
        {
            var x = exportObject.ToDynamic(metadata);

            var serializerSettings = JsonSerialization.GetDefaultSerializerSettings();
            var serializedObject = JsonConvert.SerializeObject(x, serializerSettings);

            return serializedObject;
        }
        
        public static string Serialize(object obj)
        {
            var serializerSettings = JsonSerialization.GetDefaultSerializerSettings();
            var serializedObject = JsonConvert.SerializeObject(obj, serializerSettings);

            return serializedObject;
        }
        
        public static object Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, JsonSerialization.GetDefaultSerializerSettings());
        }
    }
}