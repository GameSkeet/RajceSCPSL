using System.Collections.Generic;

namespace RajceUI.Elements
{
    public class BoxElement : ElementGroup
    {
        public BoxElement(Element element) : this(new[] { element }) { }

        public BoxElement(IEnumerable<Element> elements) : base(elements) { }
    }
}
