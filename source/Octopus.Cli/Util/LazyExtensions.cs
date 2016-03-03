using System;

// ReSharper disable CheckNamespace
namespace Octopus.Cli.Util
{
    public static class LazyExtensions
// ReSharper restore CheckNamespace
    {
        public static T LoadValue<T>(this Lazy<T> lazy)
        {
            return lazy.Value;
        }
    }
}