using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class TagResource : INamedResource
    {
        /// <summary>
        /// Gets or sets the name of this tag.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this tag.
        /// </summary>
        [Writeable]
        [Trim]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the color of this tag.
        /// </summary>
        [Writeable]
        [Trim]
        public string Color { get; set; }

        [JsonProperty(Order = -100, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// This is the canonical name for the Tag formed as {TagSetName}/{TagName} which is easier to work with than the ID in certain scenarios.
        /// </summary>
        [JsonProperty(Order = -50, NullValueHandling = NullValueHandling.Ignore)]
        public string CanonicalTagName { get; set; }

        [Writeable]
        [Trim]
        public int SortOrder { get; set; }

        public class StandardColor
        {
            public const string DarkRed = "#983230";
            public const string LightRed = "#E8634F";
            public const string DarkYellow = "#A77B22";
            public const string LightYellow = "#ECAD3F";
            public const string DarkGreen = "#227647";
            public const string LightGreen = "#36A766";
            public const string DarkCyan = "#52818C";
            public const string LightCyan = "#77B7C5";
            public const string DarkBlue = "#203A88";
            public const string LightBlue = "#3156B3";
            public const string DarkPurple = "#752BA5";
            public const string LightPurple = "#9786A7";
            public const string DarkGrey = "#6e6e6e";
            public const string LightGrey = "#9d9d9d";
        }
    }
}