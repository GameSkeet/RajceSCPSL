using System.Collections.Generic;

namespace RajceUI.Elements
{
    public class IndentElement : ElementGroup
    {
        public readonly int level;
        protected IndentElement() { }

        public IndentElement(IEnumerable<Element> elements, int level = 1) : base(elements)
        {
            this.level = level;
        }
    }
}
