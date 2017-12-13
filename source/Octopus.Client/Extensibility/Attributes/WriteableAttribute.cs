using System;

namespace Octopus.Client.Extensibility.Attributes
{
    /// <summary>
    /// Properties with this attribute will be persisted to the server when sent using a POST or PUT request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class WriteableAttribute : ApiPropertyAttribute
    {
    }

    /// <summary>
    /// Properties with this attribute will be persisted to the server when sent using a POST request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class WriteableOnCreateAttribute : ApiPropertyAttribute
    {
    }

}