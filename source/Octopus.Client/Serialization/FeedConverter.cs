using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class FeedConverter : InheritedClassConverter<FeedResource, FeedType>
    {
        static readonly IDictionary<FeedType, Type> FeedTypeMappings =
            new Dictionary<FeedType, Type>
            {
                {FeedType.NuGet, typeof(NuGetFeedResource)},
                {FeedType.Docker, typeof(DockerFeedResource)}
            };

        protected override IDictionary<FeedType, Type> DerivedTypeMappings => FeedTypeMappings;
        protected override string TypeDesignatingPropertyName => "FeedType";
    }
}