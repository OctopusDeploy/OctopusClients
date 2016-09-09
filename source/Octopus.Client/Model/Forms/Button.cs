using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// A button is essentially an 'option' that may be associated
    /// with other form elements. <seealso cref="SubmitButtonGroup" />
    /// </summary>
    public class Button
    {
        public Button(string text, string value = null)
        {
            if (text == null) throw new ArgumentNullException("text");

            Value = value ?? text;
            Text = text;
        }

        public string Text { get; private set; }
        public object Value { get; private set; }
    }
}