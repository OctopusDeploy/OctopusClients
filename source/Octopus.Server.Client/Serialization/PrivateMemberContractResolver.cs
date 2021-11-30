using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Octopus.Client.Serialization
{
    public class PrivateMemberContractResolver: DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (!property.Writable)
            {
                var property2 = member as PropertyInfo;
                if (property2 is null) return property;

                var hasPrivateSetter = property2.GetSetMethod(true) != null;
                property.Writable = hasPrivateSetter;
            }

            return property;
        }
    }
}