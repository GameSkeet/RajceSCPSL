﻿using RajceUI.Utils;
using RajceUI.Binder;

namespace RajceUI.Elements
{
    public class IntMinMaxSliderElement : MinMaxSliderElement<int>
    {
        public IntMinMaxSliderElement(LabelElement label, IBinder<MinMax<int>> binder, SliderOption<int> option)
            : base(label, binder, option.SetMinMaxGetterIfNotExist(IntSliderElement.MinGetterDefault, IntSliderElement.MaxGetterDefault))
        {
        }
    }
}
