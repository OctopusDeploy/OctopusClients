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
    }
}