using RajceUI.Utils;
using RajceUI.Binder;

namespace RajceUI.Elements
{
    public abstract class MinMaxSliderElement<T> : SliderBaseElement<MinMax<T>, T>
    {
        public MinMaxSliderElement(LabelElement label, IBinder<MinMax<T>> binder, SliderOption<T> option)
            : base(label, binder, option)
        {
        }
    }
}
