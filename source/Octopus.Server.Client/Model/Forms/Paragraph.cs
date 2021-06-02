using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// A block of instructive text.
    /// </summary>
    public class Paragraph : Control
    {
        public Paragraph(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}