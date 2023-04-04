using RajceUI.Binder;

using System.Linq;

namespace RajceUI.Elements
{
    public abstract class ReadOnlyFieldElement<T> : ReadOnlyValueElement<T>
    {
        public LabelElement Label => Children.FirstOrDefault() as LabelElement;

        protected ReadOnlyFieldElement(LabelElement label, IGetter<T> getter) : base(getter)
        {
            if (label != null)
            {
                label.SetLabelTypeToPrefixIfAuto();
                AddChild(label);
            }
        }
    }
}
