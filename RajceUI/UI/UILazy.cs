using RajceUI.Elements;

using System;

namespace RajceUI.UI
{
    public static partial class UIElements
    {
        /// <summary>
        /// Element to delay build call until Enable==true
        /// </summary>
        public static DynamicElement Lazy(Func<Element> build)
        {
            // DynamicElementのコンストラクタ内の初回Build()を抑制
            var isInConstructor = true;

            // ReSharper disable once AccessToModifiedClosure
            var ret = DynamicElementIf(() => !isInConstructor, build);

            isInConstructor = false;

            return ret;
        }
    }
}
