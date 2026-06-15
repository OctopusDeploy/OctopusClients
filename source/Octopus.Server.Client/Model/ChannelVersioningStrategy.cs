using Newtonsoft.Json;
using Octopus.TinyTypes;

namespace Octopus.Client.Model
{
    public class ChannelVersioningStrategy : CaseInsensitiveStringTinyType
    {
        public static readonly ChannelVersioningStrategy SemVer = new(nameof(SemVer));
        public static readonly ChannelVersioningStrategy MostRecentlyPublished = new(nameof(MostRecentlyPublished));

        [JsonConstructor]
        public ChannelVersioningStrategy(string value) : base(value)
        {
        }
    }
}
