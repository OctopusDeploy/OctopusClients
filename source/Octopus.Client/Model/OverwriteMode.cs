using Octopus.Client.Logging;

namespace Octopus.Client.Model
{
    public enum OverwriteMode
    {
        FailIfExists,
        OverwriteExisting,
        IgnoreIfExists
    }

    internal static class OverwriteModeLink
    {
        public static string Link = "overwriteMode";

        public static bool AsLegacyReplaceFlag(this OverwriteMode @this, ILog logger)
        {
            if (@this == OverwriteMode.IgnoreIfExists)
            {
                logger.Warn("You have selected to ignore existing versions but the Octopus server you are connected to doesn't not support this. Falling back to default value of `replace=false`");
                return false;
            }

            return @this == OverwriteMode.OverwriteExisting;
        }
    }
}