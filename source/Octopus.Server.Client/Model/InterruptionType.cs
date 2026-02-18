#nullable enable
using System;

namespace Octopus.Client.Model
{
    public class InterruptionType : IEquatable<InterruptionType>
    {
        public static readonly InterruptionType ManualIntervention = new("ManualIntervention");
        public static readonly InterruptionType GuidedFailure = new("GuidedFailure");
        public static readonly InterruptionType PullRequestCompletion = new("PullRequestCompletion");
        public static readonly InterruptionType ArgoCDApplicationSync = new("ArgoCDApplicationSync");

        public InterruptionType(string id)
        {
            Id = id;
        }

        private string Id { get; }

        public bool Equals(InterruptionType? other)
        {
            if ((object?)other == null)
                return false;
            return (object)this == (object)other ||
                   string.Equals(this.Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if ((object)this == obj)
                return true;
            return !(obj.GetType() != this.GetType()) && this.Equals((InterruptionType)obj);
        }

        public override int GetHashCode()
            => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Id);

        public static bool operator ==(InterruptionType left, InterruptionType right)
            => object.Equals((object)left, (object)right);

        public static bool operator !=(InterruptionType left, InterruptionType right)
            => !object.Equals((object)left, (object)right);

        public override string ToString()
            => Id;
    }
}
