using System.Reflection;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Tests
{
    public static class IdExtension
    {
        public static T WithId<T>(this T item, string id) where T : class, IResource
        {
            if (item == null) return null;

            var idProperty = item.GetType().GetProperty("Id", BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public);
            idProperty.SetValue(item, id, null);

            return item;
        }
    }
}