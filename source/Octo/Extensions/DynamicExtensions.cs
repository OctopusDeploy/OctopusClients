using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Octopus.Cli.Exporters;

namespace Octopus.Cli.Extensions
{
    public static class DynamicExtensions
    {
        public static dynamic ToDynamic(this object value, ExportMetadata metadata)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            if (value is IEnumerable)
            {
                var items = new List<object>();
                var values = value as IEnumerable;
                foreach (var v in values)
                {
                    items.Add(GetExpandoObject(v));
                }
                expando.Add("Items", items);
                expando.Add("$Meta", metadata);
            }
            else
            {
                expando = GetExpandoObject(value);
                expando.Add("$Meta", metadata);
            }
            return expando;
        }

        static IDictionary<string, object> GetExpandoObject(object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));
            return expando;
        }
    }
}