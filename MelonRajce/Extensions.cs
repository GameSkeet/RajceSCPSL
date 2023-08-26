using System;

using UnityEngine;

namespace MelonRajce
{
    internal static class Extensions
    {
        public static string Rep(this string s, int c)
        {
            string res = s;

            for (int i = 0; i < c; i++)
                res += s;

            return res;
        }

        public static bool IsNegative(this int i) => (i >> 31) == 1;
        public static bool IsNegative(this float f) => Math.Abs(f) != f;

        public static Vector2 ToV2(this Vector3 vec) => new Vector2(vec.x, vec.y);

        public static Rect Copy(this Rect rect) => new Rect(rect.position, rect.size);
        public static Rect FixWH(this Rect rect)
        {
            if (rect.width.IsNegative())
            {
                rect.width = Math.Abs(rect.width);
                rect.x -= rect.width;
            }
            if (rect.height.IsNegative())
            {
                rect.height = Math.Abs(rect.height);
                rect.y -= rect.height;
            }

            return rect;
        }

        public static GUIStyle Copy(this GUIStyle style) => new GUIStyle(style);
        public static void Copy(this GUISettings settings, ref GUISkin gs)
        {
            GUISettings s = gs.settings ?? new GUISettings();

            s.cursorColor = settings.cursorColor;
            s.cursorFlashSpeed = settings.cursorFlashSpeed;
            s.doubleClickSelectsWord = settings.doubleClickSelectsWord;
            s.selectionColor = settings.selectionColor;
            s.tripleClickSelectsLine = settings.tripleClickSelectsLine;
        }
        public static GUISkin Copy(this GUISkin skin)
        {
            GUISkin gs = new GUISkin();
            gs.box = skin.box.Copy();
            gs.button = skin.button.Copy();

            GUIStyle[] styles = new GUIStyle[skin.customStyles.Length];
            Array.Copy(skin.customStyles, styles, styles.Length);
            gs.customStyles = styles;

            gs.font = skin.font;
            gs.hideFlags = skin.hideFlags;
            gs.horizontalScrollbar = skin.horizontalScrollbar.Copy();
            gs.horizontalScrollbarLeftButton = skin.horizontalScrollbarLeftButton.Copy();
            gs.horizontalScrollbarRightButton = skin.horizontalScrollbarRightButton.Copy();
            gs.horizontalScrollbarThumb = skin.horizontalScrollbarThumb.Copy();
            gs.horizontalSlider = gs.horizontalSlider.Copy();
            gs.horizontalSliderThumb = skin.horizontalSliderThumb.Copy();
            gs.label = skin.label.Copy();
            gs.name = skin.name;
            gs.scrollView = skin.scrollView.Copy();
            skin.settings.Copy(ref gs);
            gs.textArea = skin.textArea.Copy();
            gs.textField = skin.textField.Copy();
            gs.toggle = skin.toggle.Copy();
            gs.verticalScrollbar = skin.verticalScrollbar.Copy();
            gs.verticalScrollbarDownButton = skin.verticalScrollbarDownButton.Copy();
            gs.verticalScrollbarThumb = skin.verticalScrollbarThumb.Copy();
            gs.verticalScrollbarUpButton = skin.verticalScrollbarUpButton.Copy();
            gs.verticalSlider = gs.verticalSlider.Copy();
            gs.verticalSliderThumb = skin.verticalSliderThumb.Copy();
            gs.window = skin.window.Copy();

            return gs;
        }

        public static Color Darken(this Color col, float by)
        {
            return new Color(Math.Max(col.r - by, 0f), Math.Max(col.g - by, 0f), Math.Max(col.b - by, 0f), col.a);
        }
        public static Color Darken32(this Color col, int by) => col.Darken(by / 255.0f);
        public static Color32 Darken(this Color32 col, int by)
        {
            return new Color32((byte)Math.Max(col.r - by, 0), (byte)Math.Max(col.g - by, 0), (byte)Math.Max(col.b - by, 0), col.a);
        }

        public static Color GetRoleColor(this CharacterClassManager ccm)
        {
            int cur = ccm.curClass;
            Team team = ccm.klasy[cur].team;

            switch (team)
            {
                case Team.SCP:
                    return Color.red;
                case Team.MTF:
                    {
                        switch (cur)
                        {
                            case 4: // SCI MTF
                            case 11: // LIE MTF
                                return new Color32(34, 130, 227, 255);

                            case 12: // COM MTF
                                return Color.blue;
                            case 13: // CAD MTF
                                return Color.cyan;
                            case 15: // GUARD
                                return Color.gray;
                        }
                    }
                    break;
                case Team.CHI:
                    return Color.green;
                case Team.RSC:
                    return Color.yellow;
                case Team.CDP:
                    return new Color32(232, 117, 9, 255);
                case Team.RIP:
                    return Color.black;
                case Team.TUT:
                    return Color.magenta;
            }

            return Color.white;
        }
    }
}
