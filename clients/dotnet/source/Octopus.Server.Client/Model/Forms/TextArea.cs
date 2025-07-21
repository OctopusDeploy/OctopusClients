using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// An input area for paragraphs of text.
    /// </summary>
    public class TextArea : Control
    {
        public TextArea(string label)
        {
            Label = label;
        }

        public string Label { get; private set; }

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