using System;

// ReSharper disable CheckNamespace
public static class LazyExtensions
// ReSharper restore CheckNamespace
{
    public static T LoadValue<T>(this Lazy<T> lazy)
    {
        return lazy.Value;
    }
}