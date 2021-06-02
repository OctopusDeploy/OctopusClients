using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// An visual component of a <see cref="Form" />.
    /// </summary>
    public abstract class Control
    {
        /// <summary>
        /// Convert a string into the native type supported
        /// by the control. Only supported if <see cref="GetNativeValueType" />
        /// returns a non-null type.
        /// </summary>
        /// <param name="value">The value to coerce. Must not be null or whitespace.</param>
        /// <returns>The value.</returns>
        public virtual object CoerceValue(string value)
        {
            throw new InvalidOperationException("The control does not support values");
        }

        /// <summary>
        /// Get the native value type supported by the control.
        /// If the value returned is null, the control does not support values.
        /// </summary>
        /// <returns>The native type, or null.</returns>
        public virtual Type GetNativeValueType()
        {
            return null;
        }
    }
}