using System;

namespace Octopus.Client.Model
{
    /// <summary>
    /// When reading from the API, only <see cref="HasValue" /> will be set, depending on
    /// whether a server-side value exists. When writing to the API, passing any non-null
    /// <see cref="NewValue" /> will set the value; otherwise, leaving <see cref="HasValue" />
    /// with the status quo value will cause the server-side value to be unchanged, while
    /// setting <see cref="HasValue" /> to false will clear any value.
    /// </summary>
    public class SensitiveValue : IEquatable<SensitiveValue>
    {
        public bool HasValue { get; set; }
        public string NewValue { get; set; }

        public static implicit operator SensitiveValue(string newValue)
        {
            return new SensitiveValue {HasValue = newValue != null, NewValue = newValue };
        }

        public bool Equals(SensitiveValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.HasValue, HasValue) && Equals(other.NewValue, NewValue);
        }
    }
}