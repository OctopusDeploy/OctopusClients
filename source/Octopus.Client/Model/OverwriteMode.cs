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

        public static bool ConvertToLegacyReplaceFlag(this OverwriteMode @this, ILog logger)
        {
            if (@this == OverwriteMode.IgnoreIfExists)
            {
                logger.Warn("The option to ignore existing versions is only supported by Octopus Server 2019.7.9 or newer. If you want to use this option please upgrade your Octopus Server. In the meantime we are automatically falling back to the default option `replace=false` for this request.");
                return false;
            }

            return @this == OverwriteMode.OverwriteExisting;
        }
    }
}