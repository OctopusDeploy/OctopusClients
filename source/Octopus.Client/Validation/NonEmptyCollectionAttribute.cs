using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Octopus.Client.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonEmptyCollectionAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var collection = (IEnumerable)(value ?? new string[0]);
            return collection.OfType<object>().Any(v => v != null && !string.IsNullOrWhiteSpace(v.ToString()));
        }
    }
}