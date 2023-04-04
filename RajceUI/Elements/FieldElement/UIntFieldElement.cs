using RajceUI.Binder;

namespace RajceUI.Elements
{
    public class UIntFieldElement : FieldBaseElement<uint>
    {
        public UIntFieldElement(LabelElement label, IBinder<uint> binder, FieldOption option) : base(label, binder, option)
        { }
    }
}
