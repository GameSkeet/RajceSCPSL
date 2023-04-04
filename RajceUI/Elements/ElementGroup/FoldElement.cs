using RajceUI.Reactive;

using System.Collections.Generic;

namespace RajceUI.Elements
{
    public class FoldElement : OpenCloseBaseElement
    {
        public FoldElement(Element header, IEnumerable<Element> contents) : base(header, contents)
        {
        }

        public override ReactiveProperty<bool> IsOpenRx { get; } = new();
    }
}
