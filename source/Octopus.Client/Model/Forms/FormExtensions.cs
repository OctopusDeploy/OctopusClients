using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model.Forms
{
    public static class FormExtensions
    {
        public static void AddElement(this Form form, string name, Control element, string initialValue = null, bool isValueRequired = false)
        {
            if (form == null) throw new ArgumentNullException("form");
            if (name == null) throw new ArgumentNullException("name");
            if (element == null) throw new ArgumentNullException("element");
            if (form.Elements.Any(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("The form already contains an element '" + name + "'");

            form.Elements.Add(new FormElement(name, element, isValueRequired));
            form.Values.Add(name, initialValue);
        }

        public static void SetValue(this Form form, string name, string value)
        {
            if (form == null) throw new ArgumentNullException("form");
            if (name == null) throw new ArgumentNullException("name");
            form.GetElement(name);
            form.Values[name] = value;
        }

        static FormElement GetElement(this Form form, string name)
        {
            if (form == null) throw new ArgumentNullException("form");
            var element = form.Elements.SingleOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (element == null)
                throw new InvalidOperationException("The form does not contain element '" + name + "'");
            return element;
        }

        static string GetRawValue(this Form form, string name)
        {
            if (form == null) throw new ArgumentNullException("form");
            form.GetElement(name);
            string value;
            form.Values.TryGetValue(name, out value);
            return value;
        }

        public static object GetCoercedValue(this Form form, string name)
        {
            var element = form.GetElement(name);
            var value = form.GetRawValue(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (element.IsValueRequired) throw new ArgumentException("The required value " + name + " is not present");
                return null;
            }
            return element.Control.CoerceValue(value);
        }

        public static void UpdateValues(this Form form, IDictionary<string, string> values)
        {
            if (form == null) throw new ArgumentNullException("form");
            if (values == null) throw new ArgumentNullException("values");
            form.Values.Clear();
            foreach (var value in values)
            {
                form.SetValue(value.Key, value.Value);
            }
        }

        public static IList<string> Validate(this Form form)
        {
            if (form == null) throw new ArgumentNullException("form");

            var result = new List<string>();

            foreach (var element in form.Elements)
            {
                var valueType = element.Control.GetNativeValueType();
                var value = form.GetRawValue(element.Name);
                var valueProvided = !string.IsNullOrWhiteSpace(value);

                if (valueType == null)
                {
                    if (valueProvided)
                        result.Add("Values are not supported for " + element.Name);

                    continue;
                }

                if (!valueProvided)
                {
                    if (element.IsValueRequired)
                        result.Add("A value is required for " + element.Name);

                    continue;
                }

                // A bit dubious, but also the only bulletproof way to know that
                // a value provided up front will be coercible later.
                try
                {
                    element.Control.CoerceValue(value);
                }
                catch
                {
                    result.Add("The value '" + value + "' is not in the correct format (" + valueType.Name + " expected)");
                }
            }

            return result;
        }
    }
}