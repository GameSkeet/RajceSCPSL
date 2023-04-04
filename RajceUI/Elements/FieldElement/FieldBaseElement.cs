﻿using RajceUI.Binder;

namespace RajceUI.Elements
{
    public struct FieldOption
    {
        public static FieldOption Default => new();
        public bool delayInput;
    }

    public abstract class FieldBaseElement<T> : ReadOnlyFieldElement<T>
    {
        public FieldOption Option { get; protected set; }
        private readonly IBinder<T> _binder;

        protected FieldBaseElement(LabelElement label, IBinder<T> binder, in FieldOption option = default) : base(label, binder)
        {
            _binder = binder;
            Option = option;
            Interactable = !binder.IsReadOnly;
        }

        protected override ElementViewBridge CreateViewBridge() => new FieldViewBridgeBase(this);

        public class FieldViewBridgeBase : ReadOnlyValueViewBridgeBase
        {
            private FieldBaseElement<T> Element => (FieldBaseElement<T>)element;

            public FieldViewBridgeBase(FieldBaseElement<T> element) : base(element)
            {
            }

            public void SetValueFromView(T t)
            {
                Element._binder?.Set(t);
                Element.NotifyViewValueChanged();
            }
        }
    }


    public static partial class ElementViewBridgeExtensions
    {
        public static FieldBaseElement<T>.FieldViewBridgeBase GetViewBridge<T>(this FieldBaseElement<T> element) => (FieldBaseElement<T>.FieldViewBridgeBase)element.ViewBridge;
    }
}
