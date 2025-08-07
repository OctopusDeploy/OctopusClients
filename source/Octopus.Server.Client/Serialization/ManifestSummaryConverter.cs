using System;
using System.Collections.Generic;
using Octopus.Client.Model.Observability;
using Octopus.Client.Model.Observability.LiveStatus;

namespace Octopus.Client.Serialization
{
    public class ManifestSummaryConverter : InheritedClassConverter<ManifestSummaryResource, string>
    {
        static readonly IDictionary<string, Type> ManifestSummaryTypeMappings =
            new Dictionary<string, Type>
            {
                {"Pod", typeof(PodManifestSummaryResource)}
            };

        protected override IDictionary<string, Type> DerivedTypeMappings => ManifestSummaryTypeMappings;
        protected override string TypeDesignatingPropertyName => "Kind";
        protected override Type DefaultType => typeof(ManifestSummaryResource);
    }
}