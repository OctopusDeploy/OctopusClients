namespace Octopus.Client.Tests.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveNewlines(this string str)
        {
            return str?.Replace("\r", "").Replace("\n", "");
        }
    }
}