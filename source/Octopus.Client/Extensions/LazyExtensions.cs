using System;

// ReSharper disable CheckNamespace

namespace Octopus.Client.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Lazy{T}" /> instances.
    /// </summary>
    static class LazyExtensions
// ReSharper restore CheckNamespace
    {
        /// <summary>
        /// Forces the Lazy value to be loaded.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="lazy">The lazy instance.</param>
        /// <returns>The value of the lazy instance.</returns>
        public static T LoadValue<T>(this Lazy<T> lazy)
        {
            return lazy.Value;
        }
    }
}