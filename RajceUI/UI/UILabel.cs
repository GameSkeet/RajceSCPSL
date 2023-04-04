using RajceUI.Elements;

using System;

namespace RajceUI.UI
{
    public static partial class UIElements
    {
        public static LabelElement Label(LabelElement label, LabelType labelType = LabelType.Auto)
        {
            label.labelType = labelType;
            return label;
        }

        public static LabelElement Label(Func<string> readLabel, LabelType labelType = LabelType.Auto) =>
            Label((LabelElement)readLabel, labelType);
    }
}
