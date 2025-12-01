using System;
using System.Collections.Generic;
using Octopus.Client.Model.SpaceDefaultRetentionPolicies;

namespace Octopus.Client.Serialization;

public class SpaceDefaultRetentionPolicyResourceConverter : InheritedClassConverter<SpaceDefaultRetentionPolicyResource, RetentionType>
{
    static readonly IDictionary<RetentionType, Type> RetentionTypeMappings =
        new Dictionary<RetentionType, Type>
        {
            { RetentionType.LifecycleRelease, typeof(SpaceDefaultLifecycleReleaseRetentionPolicyResource) },
            { RetentionType.LifecycleTentacle, typeof(SpaceDefaultLifecycleTentacleRetentionPolicyResource) },
            { RetentionType.RunbookRetention, typeof(SpaceDefaultRunbookRetentionPolicyResource) },
        };

    protected override IDictionary<RetentionType, Type> DerivedTypeMappings => RetentionTypeMappings;
    protected override string TypeDesignatingPropertyName => nameof(SpaceDefaultRetentionPolicyResource.RetentionType);
}
