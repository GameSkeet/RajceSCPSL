﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajceUI.Elements
{
    public abstract class ElementGroupWithHeader : ElementGroup
    {
        public Element Header => _hasHeader ? Children.FirstOrDefault() : null;
        private readonly bool _hasHeader;

        public override IEnumerable<Element> Contents => _hasHeader ? Children.Skip(1) : Children;

        protected ElementGroupWithHeader(Element header, IEnumerable<Element> contents)
        {
            _hasHeader = header != null;
            SetElements(contents?.Prepend(header));
        }
    }
}
