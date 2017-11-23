using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Properties with this attribute can be set from API clients, but the value won't exist for resources returned from
    /// the server. Commonly used for fields like passwords or API
    /// keys that allow a value to be written, but not read.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotReadableAttribute : ApiPropertyAttribute
    {
    }
}