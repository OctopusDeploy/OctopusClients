using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// A Boolean option.
    /// </summary>
    public class Checkbox : Control
    {
        public Checkbox(string text)
        {
            if (text == null) throw new ArgumentNullException("text");
            Text = text;
        }

        public string Text { get; private set; }

        public override object CoerceValue(string value)
        {
            return bool.Parse(value);
        }

        public override Type GetNativeValueType()
        {
            return typeof (bool);
        }
    }
}