using RajceUI.Binder;

namespace RajceUI.Elements
{
    public class FloatFieldElement : FieldBaseElement<float>
    {
        public FloatFieldElement(LabelElement label, IBinder<float> binder, FieldOption option) : base(label, binder, option) { }
    }
}
