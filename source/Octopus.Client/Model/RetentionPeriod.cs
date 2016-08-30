using System;

namespace Octopus.Client.Model
{
    public class RetentionPeriod : IEquatable<RetentionPeriod>
    {
        readonly int quantityToKeep;
        readonly RetentionUnit unit;

        public RetentionPeriod(int quantityToKeep, RetentionUnit unit)
        {
            this.quantityToKeep = quantityToKeep;
            this.unit = unit;
        }

        public RetentionUnit Unit
        {
            get { return unit; }
        }

        public int QuantityToKeep
        {
            get { return quantityToKeep; }
        }

        public bool ShouldKeepForever
        {
            get { return QuantityToKeep == 0; }
        }

        public static RetentionPeriod KeepForever()
        {
            return new RetentionPeriod(0, RetentionUnit.Items);
        }

        public bool Equals(RetentionPeriod other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return quantityToKeep == other.quantityToKeep && unit.Equals(other.unit);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RetentionPeriod)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (quantityToKeep*397) ^ unit.GetHashCode();
            }
        }

        public static bool operator ==(RetentionPeriod left, RetentionPeriod right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RetentionPeriod left, RetentionPeriod right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return ShouldKeepForever ? "Forever" : "Last " + quantityToKeep + " " + (unit == RetentionUnit.Days ? "day" + (quantityToKeep == 1 ? "" : "s") : "item" + (quantityToKeep == 1 ? "" : "s"));
        }
    }
}