using System.Collections.Generic;

namespace RajceUI.Elements
{
    public enum ScrollViewType
    {
        Vertical,
        Horizontal,
        VerticalAndHorizontal,
    }

    public class ScrollViewElement : ElementGroup
    {
        public readonly ScrollViewType type;

        public ScrollViewElement(IEnumerable<Element> contents, ScrollViewType type = ScrollViewType.VerticalAndHorizontal) : base(contents)
        {
            this.type = type;
        }
    }
}
