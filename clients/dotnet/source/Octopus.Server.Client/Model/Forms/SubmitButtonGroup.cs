using System;
using System.Collections.Generic;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// A group of options, represented as buttons. The value of the element is the
    /// value of the selected button.
    /// </summary>
    public class SubmitButtonGroup : Control
    {
        public SubmitButtonGroup(List<Button> buttons)
        {
            if (buttons == null) throw new ArgumentNullException("buttons");
            Buttons = buttons;
        }

        public List<Button> Buttons { get; private set; }

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