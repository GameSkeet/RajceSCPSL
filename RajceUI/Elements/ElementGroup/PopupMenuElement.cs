﻿using System;
using System.Collections.Generic;


namespace RajceUI.Elements
{
    public class MenuItem
    {
        public string name;
        public bool isChecked;
        public bool isEnable = true;
        public Action action;

        public MenuItem()
        {
        }

        public MenuItem(string name, Action action)
        {
            this.name = name;
            this.action = action;
        }
    }

    public class PopupMenuElement : ElementGroup
    {
        public Func<IEnumerable<MenuItem>> CreateMenuItems { get; }

        public PopupMenuElement(Element element, Func<IEnumerable<MenuItem>> createMenuItems) : base(new[] { element })
        {
            CreateMenuItems = createMenuItems;
        }
    }
}
