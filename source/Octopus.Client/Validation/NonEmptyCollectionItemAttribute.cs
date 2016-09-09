using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Octopus.Client.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonEmptyCollectionItemAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var collection = ((IEnumerable)(value ?? new string[0])).OfType<object>().ToArray();
            return collection.Length == 0 || collection.Any(v => v != null && !string.IsNullOrWhiteSpace(v.ToString()));
        }
    }
}