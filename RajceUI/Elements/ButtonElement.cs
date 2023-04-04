﻿using RajceUI.Binder;

using System;

namespace RajceUI.Elements
{
    public class ButtonElement : ReadOnlyValueElement<string>
    {
        public event Action onClick;

        public ButtonElement(IGetter<string> readName = null, Action onClick = null) : base(readName)
        {
            this.onClick += onClick;
        }

        public void OnClick() => onClick?.Invoke();
    }
}
