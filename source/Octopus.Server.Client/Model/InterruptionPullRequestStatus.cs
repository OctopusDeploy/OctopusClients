#nullable enable
using System;

namespace Octopus.Client.Model;

public class InterruptionPullRequestStatus(string value) : IEquatable<InterruptionPullRequestStatus>
{
    public static readonly InterruptionPullRequestStatus Unknown = new("Unknown");
    public static readonly InterruptionPullRequestStatus Open = new("Open");
    public static readonly InterruptionPullRequestStatus Merged = new("Merged");
    public static readonly InterruptionPullRequestStatus Closed = new("Closed");
    public static readonly InterruptionPullRequestStatus UnknownGitVendor = new("UnknownGitVendor");

    string Value { get; } = value;

    public bool Equals(InterruptionPullRequestStatus? other)
    {
        if ((object?)other is null)
        {
            return false;
        }
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((InterruptionPullRequestStatus)obj);
    }

    public static bool operator ==(InterruptionPullRequestStatus left, InterruptionPullRequestStatus right) => Equals(left, right);
    public static bool operator !=(InterruptionPullRequestStatus left, InterruptionPullRequestStatus right) => !(left == right);

    public override int GetHashCode() => (Value != null ? Value.GetHashCode() : 0);

    public override string ToString() => Value;
}
