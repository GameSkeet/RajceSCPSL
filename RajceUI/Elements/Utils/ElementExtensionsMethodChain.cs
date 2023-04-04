﻿using System;
using System.Linq;

using UnityEngine;

namespace RajceUI.Elements
{
    public static class ElementExtensionsMethodChain
    {
        public static T SetEnable<T>(this T e, bool enable)
            where T : Element
        {
            e.Enable = enable;
            return e;
        }

        public static T SetInteractable<T>(this T e, bool interactable)
            where T : Element
        {
            e.Interactable = interactable;
            return e;
        }

        public static T SetWidth<T>(this T element, float? width)
            where T : Element
        {
            element.Style.Width = width;
            return element;
        }

        public static T SetHeight<T>(this T element, float? height)
            where T : Element
        {
            element.Style.Height = height;
            return element;
        }

        public static T SetMinWidth<T>(this T element, float? minWidth)
            where T : Element
        {
            element.Style.MinWidth = minWidth;
            return element;
        }

        public static T SetMinHeight<T>(this T element, float? minHeight)
            where T : Element
        {
            element.Style.MinHeight = minHeight;
            return element;
        }

        public static T SetMaxWidth<T>(this T element, float? maxWidth)
            where T : Element
        {
            element.Style.MaxWidth = maxWidth;
            return element;
        }

        public static T SetMaxHeight<T>(this T element, float? maxHeight)
            where T : Element
        {
            element.Style.MaxHeight = maxHeight;
            return element;
        }

        public static T SetColor<T>(this T element, Color? color)
            where T : Element
        {
            element.Style.Color = color;
            return element;
        }

        public static T SetBackgroundColor<T>(this T element, Color? color)
            where T : Element
        {
            element.Style.BackgroundColor = color;
            return element;
        }

        public static T RegisterValueChangeCallback<T>(this T element, Action onValueChanged)
            where T : Element
        {
            element.onViewValueChanged += onValueChanged;
            return element;
        }

        public static T UnregisterValueChangeCallback<T>(this T element, Action onValueChanged)
            where T : Element
        {
            element.onViewValueChanged -= onValueChanged;
            return element;
        }


        public static T RegisterUpdateCallback<T>(this T element, Action<Element> onUpdate)
            where T : Element
        {
            element.onUpdate += onUpdate;
            return element;
        }

        public static T UnregisterUpdateCallback<T>(this T element, Action<Element> onUpdate)
            where T : Element
        {
            element.onUpdate -= onUpdate;
            return element;
        }

        public static T SetOpenFlag<T>(this T element, bool flag, bool recursive = false)
            where T : Element
        {
            if (element is WindowElement windowElement)
            {
                // ビルドしていないWindowを手動でOpenしたときはGlobalBuild()でBuildする
                if (flag && !windowElement.HasBuilt)
                {
                    RajceUI.GlobalBuild(windowElement);
                }
            }

            var openCloseElements = element.Query<OpenCloseBaseElement>();
            if (!recursive)
            {
                openCloseElements = openCloseElements.Take(1);
            }

            foreach (var e in openCloseElements)
            {
                e.IsOpen = flag;
            }

            return element;
        }

        public static T Open<T>(this T element, bool recursive = false) where T : Element
            => element.SetOpenFlag(true);

        public static T Close<T>(this T element, bool recursive = false) where T : Element
            => element.SetOpenFlag(false);

        public static WindowElement SetClosable(this WindowElement windowElement, bool closable)
        {
            windowElement.Closable = closable;
            return windowElement;
        }

        public static WindowElement SetPosition(this WindowElement windowElement, Vector2? position)
        {
            windowElement.Position = position;
            return windowElement;
        }
    }
}
