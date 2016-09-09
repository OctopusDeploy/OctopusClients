using System;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// An item displayed or retrieved from a <see cref="Form" />.
    /// </summary>
    public class FormElement
    {
        public FormElement(string name, Control control, bool isValueRequired = false)
        {
            Name = name;
            Control = control;
            IsValueRequired = isValueRequired;
        }

        /// <summary>
        /// The name of the element. Must be unique within the form.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A control used to render the element.
        /// </summary>
        public Control Control { get; private set; }

        /// <summary>
        /// If true, the receiver of the form expects that a value will be
        /// provided for the element.
        /// </summary>
        public bool IsValueRequired { get; private set; }
    }
}