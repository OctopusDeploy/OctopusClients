#nullable enable
using System;

namespace Octopus.Client.Model
{
    public class PreconditionType : IEquatable<PreconditionType>
    {
        public static readonly PreconditionType ApprovalPrecondition = new("Approval");
        public PreconditionType(string id)
        {
            Id = id;
        }

        private string Id { get; }

        public bool Equals(PreconditionType? other)
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
            return !(obj.GetType() != this.GetType()) && this.Equals((PreconditionType)obj);
        }

        public override int GetHashCode()
            => StringComparer.OrdinalIgnoreCase.GetHashCode(this.Id);

        public static bool operator ==(PreconditionType left, PreconditionType right)
            => object.Equals((object)left, (object)right);

        public static bool operator !=(PreconditionType left, PreconditionType right)
            => !object.Equals((object)left, (object)right);

        public override string ToString()
            => Id;
    }
}
