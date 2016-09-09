using System;

namespace Octopus.Client.Validation
{
    // TODO: Move this to the server and use FluentValidator
    [AttributeUsage(AttributeTargets.Property)]
    public class PathSegmentAttribute : Attribute //ValidationAttribute
    {
    }
}