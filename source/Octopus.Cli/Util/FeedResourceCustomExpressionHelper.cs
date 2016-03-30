using Octopus.Client.Model;

namespace Octopus.Cli.Util
{
    /// <summary>
    /// Helps with situations where feeds use custom expressions Eg. #{MyCustomFeedURL}
    /// </summary>
    public static class FeedResourceCustomExpressionHelper
    {
        public static string FeedName = "Custom expression";

        public static FeedResource FeedResourceWithId(string id)
        {
            var feed = new FeedResource()
            {
                Id = id,
                Name = FeedResourceCustomExpressionHelper.FeedName
            };
            return feed;
        }

        /// <summary>
        /// Helps to check for a valid repository-based feed Id.
        /// 
        /// Feeds may have custom expressions as their Id, which may contain Octostache variable syntax #{MyCustomFeedURL}.
        /// If you pass a custom expression Id like this to the API, it will resolve to /api/feeds/, return all the feeds, then
        /// attempt to cast that response to a FeedResource object and you'll end up getting an empty FeedResource object instead 
        /// of null. This method helps you detect valid repository feed objects before running into this confusing API scenario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsValidRepositoryId(string id)
        {
            if (id.ToLowerInvariant().StartsWith("feeds-"))
                return true;
            return false;
        }
    }
}
