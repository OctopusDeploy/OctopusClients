using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Util
{
    static class LinkSpaceExtension
    {
        public static string AppendSpaceId(this Href link, SpaceResource space)
        {
            return $"{link}/{space.Id}";
        }
    }
}