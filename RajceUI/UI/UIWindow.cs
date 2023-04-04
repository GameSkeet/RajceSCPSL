using RajceUI.Elements;

using System.Linq;
using System.Collections.Generic;

namespace RajceUI.UI
{
    public static partial class UIElements
    {
        public static WindowElement Window(params Element[] elements) => Window(null, elements);
        public static WindowElement Window(IEnumerable<Element> elements) => Window(null, elements);

        public static WindowElement Window(LabelElement title, params Element[] elements) => new(title, elements.AsEnumerable());
        public static WindowElement Window(LabelElement title, IEnumerable<Element> elements) => new(title, elements);
    }
}
