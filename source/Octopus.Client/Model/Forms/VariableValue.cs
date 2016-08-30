using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// An input area for variable values.
    /// </summary>
    public class VariableValue : Control
    {
        public VariableValue(string name, string label, string description, bool isSecure)
        {
            Name = name ?? label;
            Label = label;
            Description = description;
            IsSecure = isSecure;
        }

        public string Name { get; private set; }
        public string Label { get; private set; }
        public string Description { get; private set; }
        public bool IsSecure { get; private set; }

        public override object CoerceValue(string value)
        {
            return value.Trim();
        }

        public override Type GetNativeValueType()
        {
            return typeof (string);
        }
    }
}