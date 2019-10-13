using System;
using System.Collections.Generic;
using System.Text;
using HeadlessChicken.Core.Enums;

namespace HeadlessChicken.Core.Actions
{
    public struct ElementSelector
    {
        public ElementSelectorType Type { get; set; }
        public string Value { get; set; }

        public ElementSelector(ElementSelectorType type, string value)
        {
            Type = type;
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
