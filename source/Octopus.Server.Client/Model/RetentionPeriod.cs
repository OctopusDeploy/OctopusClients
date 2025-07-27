using System;
using Newtonsoft.Json;
using Octopus.TinyTypes;

namespace Octopus.Client.Model
{
    public class RetentionPeriod : IEquatable<RetentionPeriod>
    {
        [JsonConstructor]
        public RetentionPeriod(RetentionPeriodStrategy strategy, int quantityToKeep, RetentionUnit unit)
        {
            Strategy = strategy ?? SelectStrategyBasedOnSettings(quantityToKeep, unit);
            QuantityToKeep = quantityToKeep;
            Unit = unit;
        }

        public RetentionPeriod(int quantityToKeep, RetentionUnit unit) : this(null, quantityToKeep, unit)
        {}

        public RetentionPeriodStrategy Strategy { get; protected set; }

        public RetentionUnit Unit { get; protected set; }

        public int QuantityToKeep { get; protected set; }

        public bool ShouldKeepForever => QuantityToKeep == 0;

        public static RetentionPeriod Default() => new(RetentionPeriodStrategy.Default, 0, RetentionUnit.Items);

        RetentionPeriodStrategy SelectStrategyBasedOnSettings(int quantityToKeep, RetentionUnit unit)
        {
            return quantityToKeep == 0 && unit == RetentionUnit.Items
                ? Strategy = RetentionPeriodStrategy.Default
                : Strategy = RetentionPeriodStrategy.Count;
        }

        public bool Equals(RetentionPeriod other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Strategy.Equals(other.Strategy) && QuantityToKeep == other.QuantityToKeep && Unit.Equals(other.Unit);
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
                var hashCode = Strategy.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Unit;
                hashCode = (hashCode * 397) ^ QuantityToKeep;
                return hashCode;
            }
        }

        public static bool operator ==(RetentionPeriod left, RetentionPeriod right) => Equals(left, right);

        public static bool operator !=(RetentionPeriod left, RetentionPeriod right) => !Equals(left, right);

        public override string ToString() => ShouldKeepForever ? "Forever" : "Last " + QuantityToKeep + " " + (Unit == RetentionUnit.Days ? "day" + (QuantityToKeep == 1 ? "" : "s") : "item" + (QuantityToKeep == 1 ? "" : "s"));

    }

    public class RetentionPeriodStrategy(string value) : CaseInsensitiveStringTinyType(value)
    {
        public static readonly RetentionPeriodStrategy Default = new("Default");
        public static readonly RetentionPeriodStrategy Count = new("Count");
    }
}