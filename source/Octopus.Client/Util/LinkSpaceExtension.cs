using Octopus.Client.Extensibility;

namespace Octopus.Client.Util
{
    static class LinkSpaceExtension
    {
        public static string AppendSpaceId(this Href link, string spaceId)
        {
            return AppendSpaceId(link.ToString(), spaceId);
        }

        public static string AppendSpaceId(this string link, string spaceId)
        {
            if (!string.IsNullOrEmpty(spaceId))
            {
                link += $"/{spaceId}";
            }
            return link;
        }
    }
}