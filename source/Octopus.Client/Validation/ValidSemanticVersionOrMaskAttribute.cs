using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Model;

namespace Octopus.Client.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidSemanticVersionOrMaskAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var text = (value ?? string.Empty).ToString();
            if (text.Length == 0)
                return true;

            SemanticVersion version;
            return SemanticVersion.TryParse(text, out version)
                || SemanticVersionMask.IsMask(text);
        }
    }
}