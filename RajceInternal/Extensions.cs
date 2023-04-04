using System;

using UnityEngine;

namespace RajceInternal
{
    internal static class Extensions
    {
        public static void Write(this Color color, string text, bool AsBackground = false)
        {
            string colStr = $"\x1b[{(AsBackground ? 48 : 32)};2;";
            colStr += Math.Floor(color.r * 255) + ";";
            colStr += Math.Floor(color.g * 255) + ";";
            colStr += Math.Floor(color.b * 255) + "m";

            Console.Write("{0}{1}{2}", colStr, text, "\x1b[0m");
        }
        public static void Write(this Color32 color, string text, bool AsBackground = false)
        {
            string colStr = $"\x1b[{(AsBackground ? 48 : 32)};2;";
            colStr += color.r + ";";
            colStr += color.g + ";";
            colStr += color.b + "m";

            Console.Write("{0}{1}{2}", colStr, text, "\x1b[0m");
        }

        public static void WriteLine(this Color color, string text, bool AsBackground = false)
        {
            string colStr = $"\x1b[{(AsBackground ? 48 : 32)};2;";
            colStr += Math.Floor(color.r * 255) + ";";
            colStr += Math.Floor(color.g * 255) + ";";
            colStr += Math.Floor(color.b * 255) + "m";

            Console.WriteLine("{0}{1}{2}", colStr, text, "\x1b[0m");
        }
        public static void WriteLine(this Color32 color, string text, bool AsBackground = false)
        {
            string colStr = $"\x1b[{(AsBackground ? 48 : 32)};2;";
            colStr += color.r + ";";
            colStr += color.g + ";";
            colStr += color.b + "m";

            Console.WriteLine("{0}{1}{2}", colStr, text, "\x1b[0m");
        }

        public static Vector3 ToV3(this Vector2 vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }
        public static Vector2 ToV2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector2 CalcSize(this string str, int size)
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = size
            }.CalcSize(new GUIContent(str));
        }
        public static Rect GetAsSize(this Rect rect, Vector2 vec)
        {
            return new Rect(rect.x, rect.y, vec.x, vec.y);
        }
        public static Rect GetAsSize(this Rect rect, float width, float height)
        {
            return new Rect(rect.x, rect.y, width, height);
        }
        public static Rect GetNew(this Rect rect, bool addWidth = true, bool addHeight = true)
        {
            return new Rect(rect.x + (addWidth ? rect.width : 0), rect.y + (addHeight ? rect.height : 0), 0, 0);
        }
        public static Rect Copy(this Rect rect) => new Rect(rect.position, rect.size);

        public static GUIStyle Copy(this GUIStyle style)
        {
            return new GUIStyle(style);
        }
        public static GUISkin Copy(this GUISkin skin)
        {
            GUISkin nskin = new GUISkin()
            {
                box = skin.box.Copy(),
                button = skin.button.Copy(),
                customStyles = skin.customStyles,
                font = skin.font,
                hideFlags = skin.hideFlags,
                horizontalScrollbar = skin.horizontalScrollbar.Copy(),
                horizontalScrollbarLeftButton = skin.horizontalScrollbarLeftButton.Copy(),
                horizontalScrollbarRightButton = skin.horizontalScrollbarRightButton.Copy(),
                horizontalScrollbarThumb = skin.horizontalScrollbarThumb.Copy(),
                horizontalSlider = skin.horizontalSlider.Copy(),
                horizontalSliderThumb = skin.horizontalSliderThumb.Copy(),
                label = skin.label.Copy(),
                name = skin.name,
                scrollView = skin.scrollView.Copy(),
                textArea = skin.textArea.Copy(),
                textField = skin.textField.Copy(),
                toggle = skin.toggle.Copy(),
                verticalScrollbar = skin.verticalScrollbar.Copy(),
                verticalScrollbarDownButton = skin.verticalScrollbarDownButton.Copy(),
                verticalScrollbarThumb = skin.verticalScrollbarThumb.Copy(),
                verticalScrollbarUpButton = skin.verticalScrollbarUpButton.Copy(),
                verticalSlider = skin.verticalSlider.Copy(),
                verticalSliderThumb = skin.verticalSliderThumb.Copy(),
                window = skin.window.Copy(),
            };

            return skin;
        }

        public static Color DarkenBy(this Color col, float intensity)
        {
            float offset = intensity / 255f;
            return new Color(Math.Max(col.r - offset, 0), Math.Max(col.g - offset, 0), Math.Max(col.b - offset, 0));
        }
    }
}
