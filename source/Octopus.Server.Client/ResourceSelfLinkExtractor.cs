using Octopus.Client.Extensibility;

namespace Octopus.Client
{
    internal class ResourceSelfLinkExtractor
    {
        /// <summary>
        ///     Returns the "Self" link from the provided resource if available, otherwise returns <c>null</c> 
        /// </summary>
        public string GetSelfUrlOrNull<TResource>(TResource resource)
        {
            const string self = "Self";
            if (resource is IResource res && 
                res.Links != null &&
                res.Links.ContainsKey(self))
            {
                return res.Links[self].AsString();
            }

            return null;
        }
    }
}