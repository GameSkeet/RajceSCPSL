﻿using RajceUI.Binder;

namespace RajceUI.Elements
{
    public abstract class SliderElement<T> : SliderBaseElement<T, T>
    {
        public SliderElement(LabelElement label, IBinder<T> binder, SliderOption<T> option)
            : base(label, binder, option)
        {
        }
    }

    public abstract class SliderBaseElement<T, TRange> : RangeFieldElement<T, TRange>
    {
        public readonly bool showInputField;

        public SliderBaseElement(LabelElement label, IBinder<T> binder, SliderOption<TRange> option)
            : base(label, binder, option.minGetter, option.maxGetter)
        {
            this.showInputField = option.showInputField;
        }
    }


    public class SliderOption
    {
        public IGetter minGetter;
        public IGetter maxGetter;
        public bool showInputField = true;

        public SliderOption()
        {
        }

        public SliderOption(SliderOption other)
        {
            minGetter = other.minGetter;
            minGetter = other.minGetter;
            showInputField = other.showInputField;
        }

        public SliderOption<T> Cast<T>()
        {
            return new SliderOption<T>()
            {
                minGetter = (IGetter<T>)minGetter,
                maxGetter = (IGetter<T>)maxGetter,
                showInputField = showInputField
            };
        }
    }

    public class SliderOption<T>
    {
        public IGetter<T> minGetter;
        public IGetter<T> maxGetter;
        public bool showInputField = true;
    }

    public static class SliderOptionExtension
    {
        public static SliderOption<T> SetMinMaxGetterIfNotExist<T>(this SliderOption<T> option, IGetter<T> minGetter, IGetter<T> maxGetter)
        {
            option.minGetter ??= minGetter;
            option.maxGetter ??= maxGetter;

            return option;
        }
    }
}
