using Octopus.Client.Extensibility;

namespace Octopus.Client.Util
{
    static class LinkSpaceExtension
    {
        public static string AppendSpaceId(this Href link, string spaceId)
        {
            return $"{link}/{spaceId}";
        }
    }
}