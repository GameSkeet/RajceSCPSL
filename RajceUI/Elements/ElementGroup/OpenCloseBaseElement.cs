using RajceUI.Reactive;

using System.Collections.Generic;

namespace RajceUI.Elements
{
    public abstract class OpenCloseBaseElement : ElementGroupWithHeader
    {
        public abstract ReactiveProperty<bool> IsOpenRx { get; }

        public virtual bool IsOpen
        {
            get => IsOpenRx.Value;
            set => IsOpenRx.Value = value;
        }

        protected OpenCloseBaseElement(Element header, IEnumerable<Element> contents) : base(header, contents)
        {
        }
    }
}
