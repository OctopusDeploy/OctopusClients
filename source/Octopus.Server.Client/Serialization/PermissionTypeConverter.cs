#nullable enable
using System;
using System.ComponentModel;
using System.Globalization;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class PermissionTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Permission);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string).IsAssignableFrom(sourceType)) return true;
            return false;
        }

        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object? value)
        {
            return value == null ? null : new Permission((string)value);
        }
    }
}