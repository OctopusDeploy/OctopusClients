using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class AutoDeployReleaseOverrideResource
    {
        public string EnvironmentId { get; }
        public string TenantId { get; }
        public string ReleaseId { get; }

        public AutoDeployReleaseOverrideResource(string environmentId, string releaseId)
            : this(environmentId, null, releaseId)
        {
        }

        [JsonConstructor]
        public AutoDeployReleaseOverrideResource(string environmentId, string tenantId, string releaseId)
        {
            EnvironmentId = environmentId;
            TenantId = tenantId;
            ReleaseId = releaseId;
        }

        sealed class EnvironmentIdTenantIdEqualityComparer : IEqualityComparer<AutoDeployReleaseOverrideResource>
        {
            public bool Equals(AutoDeployReleaseOverrideResource x, AutoDeployReleaseOverrideResource y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.EnvironmentId, y.EnvironmentId) && string.Equals(x.TenantId, y.TenantId);
            }

            public int GetHashCode(AutoDeployReleaseOverrideResource obj)
            {
                unchecked
                {
                    return ((obj.EnvironmentId?.GetHashCode() ?? 0)*397) ^ (obj.TenantId?.GetHashCode() ?? 0);
                }
            }
        }

        public static IEqualityComparer<AutoDeployReleaseOverrideResource> EnvironmentIdTenantIdComparer { get; } = new EnvironmentIdTenantIdEqualityComparer();
    }
}