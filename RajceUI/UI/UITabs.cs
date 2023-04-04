using RajceUI.Elements;

using System;
using System.Linq;
using System.Collections.Generic;

namespace RajceUI.UI
{
    public static partial class UIElements
    {
        public static TabsElement Tabs(params (string label, Element element)[] tabs) => Tabs(tabs.AsEnumerable());

        public static TabsElement Tabs(IEnumerable<(string label, Element element)> tabs)
        {
            return Tabs(tabs.Select(tab => Tab.Create(tab.Item1, tab.Item2)));
        }

        // Func<Element> call is delayed until displayed 
        public static TabsElement Tabs(params (string label, Func<Element> createElement)[] tabs)
        {
            return Tabs(tabs.Select(tab => Tab.Create(tab.label, tab.createElement)));
        }

        public static TabsElement Tabs(IEnumerable<Tab> tabs) => new(tabs);
    }
}
