using Octopus.Client.Extensibility;

namespace Octopus.Client
{
    public static class UrlHelper
    {
        /// <summary>
        ///     Returns the "Self" link from the provided resource if available, otherwise returns <c>null</c> 
        /// </summary>
        public static string GetSelfUrlOrNull<TResource>(TResource resource)
        {
            const string self = "Self";
            if (resource is IResource res && res.Links.ContainsKey(self))
            {
                return res.Links[self].AsString();
            }

            return null;
        }
    }
}