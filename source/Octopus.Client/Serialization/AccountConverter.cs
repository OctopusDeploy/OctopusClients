using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Serialization
{
    public class AccountConverter : JsonConverter
    {
        static readonly IDictionary<AccountType, Type> AccountTypes =
            new Dictionary<AccountType, Type>
            {
                {AccountType.UsernamePassword, typeof (UsernamePasswordAccountResource)},
                {AccountType.AzureSubscription, typeof (AzureSubscriptionAccountResource)},
                {AccountType.AzureServicePrincipal, typeof (AzureServicePrincipalAccountResource)},
                {AccountType.SshKeyPair, typeof (SshKeyPairAccountResource)}
            };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var property in value.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                .Where(p => p.CanRead))
            {
                writer.WritePropertyName(property.Name);
                serializer.Serialize(writer, property.GetValue(value, null));
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jo = JObject.Load(reader);
            var accountType = jo.GetValue("AccountType").ToObject<string>();
            var type = AccountTypes[(AccountType)Enum.Parse(typeof (AccountType), accountType)];
            var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var args = ctor.GetParameters().Select(p =>
                jo.GetValue(char.ToUpper(p.Name[0]) + p.Name.Substring(1))
                    .ToObject(p.ParameterType, serializer)).ToArray();
            var instance = ctor.Invoke(args);
            foreach (var prop in type
                .GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance)
                .Where(p => p.CanWrite))
            {
                var val = jo.GetValue(prop.Name);
                if (val != null)
                    prop.SetValue(instance, val.ToObject(prop.PropertyType, serializer), null);
            }
            return instance;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (AccountResource).IsAssignableFrom(objectType);
        }
    }
}