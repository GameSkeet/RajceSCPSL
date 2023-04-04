using RajceUI.Binder;

namespace RajceUI.Elements
{
    public class IntFieldElement : FieldBaseElement<int>
    {
        public IntFieldElement(LabelElement label, IBinder<int> binder, FieldOption option) : base(label, binder, option)
        {
        }
    }
}
